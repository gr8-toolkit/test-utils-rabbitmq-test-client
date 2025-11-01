using GR8Tech.Sport.TestUtils.RabbitMQClient.Contracts;
using Tests.Domain;

namespace Tests.Configuration.RabbitMQ;

public interface IRabbitConsumers
{
    IRabbitConsumer<SimpleClass> FirstDefaultExchangeQueue { get; }
    IRabbitConsumer<SimpleClass> SecondDefaultExchangeQueue { get; }
    IRabbitConsumer<SimpleClass> ThirdDefaultExchangeQueue { get; }
    IRabbitConsumer<SimpleClass> FanoutExchangeQueue { get; }
    IRabbitConsumer<SimpleClass> DirectExchangeQueue { get; }
    IRabbitConsumer<SimpleClass> TopicExchangeQueue { get; }
    void StartAll();
    void CloseAll();
}