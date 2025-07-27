namespace WorkSchedule.QueueRabbitMQ;

public interface IQueuePubSub
{
    Task<bool> ProduceMessage<T>(T obj, string queueName, string exchange);
    Task<T?> ConsumeMessage<T>(string queueName);
}
