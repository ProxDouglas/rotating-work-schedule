namespace WorkSchedule.AplicationStartup.RabbitMq;

using RabbitMQ.Client;
using WorkSchedule.QueueRabbitMQ;

public class RabbitMqStartUp
{
  private readonly string[] _defaultQueues = { "GenerationRequest", "GenerationResponse" };
  private const string DEFAULT_HOST = "localhost";
  private const int DEFAULT_PORT = 5672;
  private const string DEFAULT_USER = "guest";
  private const string DEFAULT_PASSWORD = "guest";
  private const string DEFAULT_VHOST = "/";

  public async Task<IConnection> CreateConnectionAsync(WebApplicationBuilder builder)
  {
    var factory = CreateConnectionFactory(builder);
    return await ConnectToRabbitMqAsync(factory);
  }

  private ConnectionFactory CreateConnectionFactory(WebApplicationBuilder builder)
  {
    return new ConnectionFactory
    {
      HostName = builder.Configuration["RabbitMQ:Host"] ?? DEFAULT_HOST,
      Port = int.Parse(builder.Configuration["RabbitMQ:Port"] ?? DEFAULT_PORT.ToString()),
      UserName = builder.Configuration["RabbitMQ:Username"] ?? DEFAULT_USER,
      Password = builder.Configuration["RabbitMQ:Password"] ?? DEFAULT_PASSWORD,
      VirtualHost = builder.Configuration["RabbitMQ:VirtualHost"] ?? DEFAULT_VHOST
    };
  }

  private async Task<IConnection> ConnectToRabbitMqAsync(ConnectionFactory factory)
  {
    try
    {
      return await factory.CreateConnectionAsync();
    }
    catch (Exception ex)
    {
      throw new RabbitMQConnectionException("Failed to connect to RabbitMQ", ex);
    }
  }

  public void ConfigureServices(WebApplicationBuilder builder)
  {
    var connection = CreateConnectionAsync(builder).GetAwaiter().GetResult();
    ConfigureQueues(connection);
    RegisterServices(builder, connection);
  }

  private void RegisterServices(WebApplicationBuilder builder, IConnection connection)
  {
    try
    {
      builder.Services.AddSingleton(connection);
      builder.Services.AddScoped<IQueuePubSub, QueuePubSub>();
      ConfigureRabbitMqOptions(builder);
      LogSuccess("RabbitMQ services configured successfully");
    }
    catch (Exception ex)
    {
      throw new RabbitMQConfigurationException("Failed to configure RabbitMQ services", ex);
    }
  }

  private void ConfigureRabbitMqOptions(WebApplicationBuilder builder)
  {
    builder.Services.Configure<RabbitMqConfig>(
      builder.Configuration.GetSection("RabbitMQ"));
      
    builder.Services.Configure<RabbitMqConfig>("Schedule",
      builder.Configuration.GetSection("RabbitMQ_Schedule"));
  }

  private void ConfigureQueues(IConnection connection)
  {
    foreach (string queue in _defaultQueues)
    {
      CreateQueueAsync(connection, queue, queue).GetAwaiter().GetResult();
    }
  }

  private async Task<bool> CreateQueueAsync(IConnection connection, string queueName, string exchange)
  {
    try
    {
      IChannel channel = await connection.CreateChannelAsync();
      await DeclareQueueAsync(channel, queueName);
      await DeclareExchangeAsync(channel, exchange);
      await BindQueueToExchangeAsync(channel, queueName, exchange);

      LogSuccess($"Queue {queueName} and exchange {exchange} created successfully");
      return true;
    }
    catch (Exception ex)
    {
      LogError($"Failed to create queue or exchange: {ex.Message}");
      return false;
    }
  }

  private async Task DeclareQueueAsync(IChannel channel, string queueName)
  {
    await channel.QueueDeclareAsync(
      queue: queueName,
      durable: true,
      exclusive: false,
      autoDelete: false,
      arguments: null);
  }

  private async Task DeclareExchangeAsync(IChannel channel, string exchange)
  {
    await channel.ExchangeDeclareAsync(
      exchange: exchange,
      type: ExchangeType.Direct,
      durable: true,
      autoDelete: false);
  }

  private async Task BindQueueToExchangeAsync(IChannel channel, string queueName, string exchange)
  {
    await channel.QueueBindAsync(
      queue: queueName,
      exchange: exchange,
      routingKey: queueName);
  }

  private void LogSuccess(string message) => Console.WriteLine(message);
  private void LogError(string message) => Console.WriteLine(message);
}

public class RabbitMQConnectionException : Exception
{
  public RabbitMQConnectionException(string message, Exception inner)
    : base(message, inner) { }
}

public class RabbitMQConfigurationException : Exception
{
  public RabbitMQConfigurationException(string message, Exception inner)
    : base(message, inner) { }
}

public class RabbitMqConfig
{
  public string Host { get; set; } = "localhost";
  public int Port { get; set; } = 5672;
  public string Username { get; set; } = "guest";
  public string Password { get; set; } = "guest";
  public string VirtualHost { get; set; } = "/";
  public string Queue { get; set; } = string.Empty;
}
