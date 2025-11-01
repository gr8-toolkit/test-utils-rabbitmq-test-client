using GR8Tech.Sport.TestUtils.RabbitMQClient.Contracts;
using Tests.Domain;

namespace Tests.Configuration.RabbitMQ;

public interface IRabbitPublishers
{
    IRabbitPublisher<SimpleClass> FirstDefaultExchangeQueue { get; }
    IRabbitPublisher<SimpleClass> SecondDefaultExchangeQueue { get; }
    IRabbitPublisher<SimpleClass> ThirdDefaultExchangeQueue { get; }
    IRabbitPublisher<SimpleClass> FanoutExchangeQueue { get; }
    IRabbitPublisher<SimpleClass> DirectExchangeQueue { get; }
    IRabbitPublisher<SimpleClass> TopicExchangeQueue { get; }
    void StartAll();
    void CloseAll();
}