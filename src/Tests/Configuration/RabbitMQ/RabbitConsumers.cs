using System.Collections.Concurrent;
using GR8Tech.Sport.TestUtils.RabbitMQClient.Contracts;
using GR8Tech.Sport.TestUtils.RabbitMQClient.Implementation;
using Tests.Domain;

namespace Tests.Configuration.RabbitMQ;

public class RabbitConsumers : IRabbitConsumers
{
    private readonly ConcurrentBag<object> _consumers = new();

    public IRabbitConsumer<SimpleClass> FirstDefaultExchangeQueue { get; private set; }
    public IRabbitConsumer<SimpleClass> SecondDefaultExchangeQueue { get; private set; }
    public IRabbitConsumer<SimpleClass> ThirdDefaultExchangeQueue { get; private set; }
    public IRabbitConsumer<SimpleClass> FanoutExchangeQueue { get; private set; }
    public IRabbitConsumer<SimpleClass> DirectExchangeQueue { get; private set; }
    public IRabbitConsumer<SimpleClass> TopicExchangeQueue { get; private set; }
    
    public void StartAll()
    {
        _consumers.Add(FirstDefaultExchangeQueue = new RabbitConsumer<SimpleClass>(RabbitQueues.GetInstance.FirstDefaultExchange));
        _consumers.Add(SecondDefaultExchangeQueue = new RabbitConsumer<SimpleClass>(RabbitQueues.GetInstance.SecondDefaultExchange));
        _consumers.Add(ThirdDefaultExchangeQueue = new RabbitConsumer<SimpleClass>(RabbitQueues.GetInstance.ThirdDefaultExchange));
        _consumers.Add(FanoutExchangeQueue = new RabbitConsumer<SimpleClass>(RabbitQueues.GetInstance.FanoutExchange));
        _consumers.Add(DirectExchangeQueue = new RabbitConsumer<SimpleClass>(RabbitQueues.GetInstance.DirectExchange));
        _consumers.Add(TopicExchangeQueue = new RabbitConsumer<SimpleClass>(RabbitQueues.GetInstance.TopicExchange));
        
        foreach (var consumer in _consumers) ((IRabbitConsume) consumer).StartConsume();
    }

    public void CloseAll()
    {
        foreach (var consumer in _consumers) ((IDisposable) consumer).Dispose();
    }
}
