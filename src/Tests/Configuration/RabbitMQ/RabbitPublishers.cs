using System.Collections.Concurrent;
using GR8Tech.Sport.TestUtils.RabbitMQClient.Contracts;
using GR8Tech.Sport.TestUtils.RabbitMQClient.Implementation;
using Tests.Domain;

namespace Tests.Configuration.RabbitMQ;

public class RabbitPublishers : IRabbitPublishers
{
    private readonly ConcurrentBag<object> _producers = new ();
    
    public IRabbitPublisher<SimpleClass> FirstDefaultExchangeQueue { get; private set; }
    public IRabbitPublisher<SimpleClass> SecondDefaultExchangeQueue { get; private set; }
    public IRabbitPublisher<SimpleClass> ThirdDefaultExchangeQueue { get; private set; }
    public IRabbitPublisher<SimpleClass> FanoutExchangeQueue { get; private set; }
    public IRabbitPublisher<SimpleClass> DirectExchangeQueue { get; private set;}
    public IRabbitPublisher<SimpleClass> TopicExchangeQueue { get; private set; }

    public void StartAll()
    {
        _producers.Add(FirstDefaultExchangeQueue = new RabbitPublisher<SimpleClass>(RabbitQueues.GetInstance.FirstDefaultExchange));
        _producers.Add(SecondDefaultExchangeQueue = new RabbitPublisher<SimpleClass>(RabbitQueues.GetInstance.SecondDefaultExchange));
        _producers.Add(ThirdDefaultExchangeQueue = new RabbitPublisher<SimpleClass>(RabbitQueues.GetInstance.ThirdDefaultExchange));
        _producers.Add(FanoutExchangeQueue = new RabbitPublisher<SimpleClass>(RabbitQueues.GetInstance.FanoutExchange));
        _producers.Add(DirectExchangeQueue = new RabbitPublisher<SimpleClass>(RabbitQueues.GetInstance.DirectExchange));
        _producers.Add(TopicExchangeQueue = new RabbitPublisher<SimpleClass>(RabbitQueues.GetInstance.TopicExchange));
    }
    
    public void CloseAll()
    {
        foreach (var producer in _producers)
        {
            ((IDisposable) producer).Dispose();
        }
    }
}