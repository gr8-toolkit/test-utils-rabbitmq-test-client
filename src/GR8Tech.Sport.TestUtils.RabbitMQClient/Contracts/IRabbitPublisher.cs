namespace GR8Tech.Sport.TestUtils.RabbitMQClient.Contracts;

public interface IRabbitPublisher<TValue> : IDisposable
{ 
    void PublishMessage(TValue message, string? routingKey = null);
}