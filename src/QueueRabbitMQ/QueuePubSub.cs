namespace WorkSchedule.QueueRabbitMQ;

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class QueuePubSub : IQueuePubSub
{
   private readonly IConnection _connection;
   private readonly IChannel Channel;
   private readonly CancellationTokenSource CancellationTokenSource = new();

   private AsyncEventingBasicConsumer? Consumer;
   public QueuePubSub(IConnection connection)
   {
      _connection = connection;
      var channelTask = connection.CreateChannelAsync();
      channelTask.Wait();
      this.Channel = channelTask.Result;
   }

   public async Task<bool> ProduceMessage<T>(T obj, string queueName, string exchange)
   {
      try
      {
         // Serializa o objeto para JSON
         var body = this.serializeObject(obj);

         // Publica a mensagem na fila
         await this.Channel.BasicPublishAsync(
             exchange: exchange,
             routingKey: queueName,
             body: body,
             mandatory: true);

         return true;
      }
      catch (Exception ex)
      {
         Console.WriteLine($"Erro ao enviar mensagem: {ex.Message}");
         return false;
      }
   }

   public async Task<T?> ConsumeMessage<T>(string queueName)
   {
      var result = await Channel.BasicGetAsync(queue: queueName, autoAck: true);

      if (result == null || result.Body.IsEmpty)
         return default;

      return this.deserializeObject<T>(result);
   }

   private byte[] serializeObject<T>(T obj)
   {
      var message = System.Text.Json.JsonSerializer.Serialize(obj);
      var body = System.Text.Encoding.UTF8.GetBytes(message);
      return body;
   }

   private T? deserializeObject<T>(BasicGetResult result)
   {
      var message = System.Text.Encoding.UTF8.GetString(result.Body.ToArray());
      if (string.IsNullOrEmpty(message))
         return default;
      return System.Text.Json.JsonSerializer.Deserialize<T>(message);
   }
}
