# Rabbit MQ client

Author: Oleh Remishevskyi

[[_TOC_]]

## TL;DR
This repository contains primary the source code of the NuGet package:
- GR8Tech.Sport.TestUtils.RabbitMQClient

This NuGet developed to access Redis server and interact with it.
So far the package is available in our nexus NuGet sources:
- https://nexus.pm.tech/repository/NuGet-sport-main

Here is the functionality of Rabbit MQ client.
Which give us an ultimate Library for working with Rabbit MQ

This project provides you:

* NuGets:
  * RabbitMQClient

## Main interfaces and classes

### IRabbitConsumer

Use this interface to work with Consumer.
You got class which which implements this interface:

* **RabbitConsumer<TValue> : IRabbitConsumer<TValue>**

Methods:

* `List<DataHolder<TValue>> GetMessages(Func<DataHolder<TValue>, bool> compare);`
* `List<DataHolder<TValue>> WaitAndGetMessages(Func<DataHolder<TValue>, bool> compare);;`
* `List<DataHolder<TValue>> WaitAndGetMessages(Func<DataHolder<TValue>, bool> compare, int timeoutRetries, int delaysBetweenRetries);`
* `void WaitForMessage(Func<DataHolder<TValue>, bool> compare, int timeoutRetries, int delaysBetweenRetries);`
* `void WaitForMessage(Func<DataHolder<TValue>, bool> compare);`
* `List<DataHolder<TValue>> WaitAndCheckThatMsgIsNotPresent(Func<DataHolder<TValue>, bool> compare);`
* `List<DataHolder<TValue>> WaitAndCheckThatMsgIsNotPresent(Func<DataHolder<TValue>, bool> compare, int timeoutRetries, int delaysBetweenRetries);`
* `void CheckThatMsgIsNotPresent(Func<DataHolder<TValue>, bool> compare);`
* `void CheckThatMsgIsNotPresent(Func<DataHolder<TValue>, bool> compare, int timeoutRetries, int delaysBetweenRetries);`
* `void ForceFullCleanMessagesCollection();`

### IRabbitPublisher

Use this interface to work with Producer.
You got class which implements this interface:

* **RabbitPublisher<TValue> : IRabbitPublisher<TValue>**

Methods:

* `void PublishMessage(TValue message, string? routingKey = null);`

## Settings file

To configure work with RabbitPublisher or RabbitConsumer, you have a flexible options test-settings.json.

* You can specify only consumers or only producers depend on what you need in your tests.
* You can specify default settings for consumer & producers, and skip editing each consumer or producer.
* You can specify only queue names and rest will set to default.
* You can specify PollingSettings to set custom retry policy.
* You can specify CleanUpQueueSettings to set custom clean up policy.
* You can specify EnvironmentSettings to set custom HostName and Env.

Full example of RabbitMqConfiguration section, if you need to configure everything

```json
{
  "RabbitMqConfiguration": {
    "CleanUpQueueSettings": {
      "IsCleanUpEnabled": false,
      "FromMessageCount": 200
    },
    "PollingSettings": {
      "DefaultTimeoutRetries": 30,
      "DefaultDelaysBetweenRetries": 1
    },
    "EnvironmentSettings": {
      "HostName": "localhost",
      "Env": "local"
    },
    "QueueSettings": [
      {
        "QueueSettingsName": "FirstDefaultExchangeQueue-Settings",
        "Queue": "FirstDefaultExchangeQueue-${env}",
        "ExchangeType": "",
        "ExchangeName": "",
        "RoutingKey": "FirstDefaultExchangeQueue-${env}",
        "Durable": true,
        "AutoDelete": false,
        "AutoAck": false,
        "Exclusive": false,
        "Arguments": {},
        "Bindings": []
      },
      {
        "QueueSettingsName": "SecondDefaultExchangeQueue-Settings",
        "Queue": "SecondDefaultExchangeQueue-${env}",
        "ExchangeType": "",
        "ExchangeName": "",
        "RoutingKey": "SecondDefaultExchangeQueue-${env}",
        "Durable": true,
        "AutoDelete": false,
        "AutoAck": false,
        "Exclusive": false,
        "Arguments": {},
        "Bindings": []
      },
      {
        "QueueSettingsName": "ThirdDefaultExchangeQueue-Settings",
        "Queue": "ThirdDefaultExchangeQueue-${env}",
        "ExchangeType": "",
        "ExchangeName": "",
        "RoutingKey": "ThirdDefaultExchangeQueue-${env}",
        "Durable": true,
        "AutoDelete": false,
        "AutoAck": false,
        "Exclusive": false,
        "Arguments": {},
        "Bindings": []
      },
      {
        "QueueSettingsName": "FanoutExchangeQueue-Settings",
        "Queue": "FanoutExchangeQueue-${env}",
        "ExchangeType": "Fanout",
        "ExchangeName": "FanoutExchange",
        "RoutingKey": "",
        "Durable": true,
        "AutoDelete": false,
        "AutoAck": false,
        "Exclusive": false,
        "Arguments": {},
        "Bindings": []
      },
      {
        "QueueSettingsName": "DirectExchangeQueue-Settings",
        "Queue": "DirectExchangeQueue-${env}",
        "ExchangeType": "Direct",
        "ExchangeName": "DirectExchange",
        "RoutingKey": "error",
        "Durable": true,
        "AutoDelete": false,
        "AutoAck": false,
        "Exclusive": false,
        "Arguments": {},
        "Bindings": [
          {
            "RoutingKey": "error"
          },
          {
            "RoutingKey": "warning"
          }
        ]
      },
      {
        "QueueSettingsName": "TopicExchangeQueue-Settings",
        "Queue": "TopicExchangeQueue-${env}",
        "ExchangeType": "Topic",
        "ExchangeName": "TopicExchange",
        "RoutingKey": "*.red.#",
        "Durable": true,
        "AutoDelete": false,
        "AutoAck": false,
        "Exclusive": false,
        "Arguments": {},
        "Bindings": []
      }
    ]
  }
}
```

## Examples how to set up and use

### TestServices
```csharp
public static class TestServices
{
    public static IRabbitConsumers RabbitConsumers { get; }
    public static IRabbitPublishers RabbitPublishers { get; }

    static TestServices()
    {
        RabbitConsumers = new RabbitConsumers();
        RabbitPublishers = new RabbitPublishers();
    }
}
```

### SetUp and TearDown
```csharp

[SetUpFixture]
[Parallelizable(ParallelScope.All)]
public class AssemblySetUp
{
    private ICompositeService? Docker { get; set; }
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        //start docker with rabbitmq
        Docker = new DockerDependency().Start();

        //start all consumers and producers
        TestServices.RabbitPublishers.StartAll();
        TestServices.RabbitConsumers.StartAll();
    }

    [OneTimeTearDown]
    public void BaseTearDown()
    {
        //dispose all consumers and producers
        TestServices.RabbitConsumers.CloseAll();
        TestServices.RabbitPublishers.CloseAll();
        
        //dispose docker with rabbitmq
        Docker?.Dispose();
    }
}
```
### IRabbitConsumers
```csharp
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
```
### RabbitConsumers
```csharp
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
```

### IRabbitPublishers
```csharp
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
```

### RabbitPublishers
```csharp
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
```

### RabbitQueues
```csharp
public sealed class RabbitQueues
{
    private static RabbitQueues? _queues;
    private static readonly object LockObject = new();
    
    public QueueSettings FirstDefaultExchange { get; private set; }
    public QueueSettings SecondDefaultExchange { get; private set; }
    public QueueSettings ThirdDefaultExchange { get; private set; }
    public QueueSettings FanoutExchange { get; private set; }
    public QueueSettings DirectExchange { get; private set; }
    public QueueSettings TopicExchange { get; private set; }
    
    private RabbitQueues()
    {
    }

    public static RabbitQueues GetInstance
    {
        get
        {
            if (_queues == null)
            {
                lock (LockObject)
                {
                    if (_queues == null)
                    {
                        _queues = new RabbitQueues
                        {
                            FirstDefaultExchange = RabbitMqSettingsProvider.Options.QueueSettings
                                .Single(t => t.QueueSettingsName.Contains("FirstDefaultExchangeQueue-Settings")),
                            SecondDefaultExchange = RabbitMqSettingsProvider.Options.QueueSettings
                                .Single(t => t.QueueSettingsName.Contains("SecondDefaultExchangeQueue-Settings")),
                            ThirdDefaultExchange = RabbitMqSettingsProvider.Options.QueueSettings
                                .Single(t => t.QueueSettingsName.Contains("ThirdDefaultExchangeQueue-Settings")),
                            FanoutExchange = RabbitMqSettingsProvider.Options.QueueSettings
                                .Single(t => t.QueueSettingsName.Contains("FanoutExchangeQueue-Settings")),
                            DirectExchange = RabbitMqSettingsProvider.Options.QueueSettings
                                .Single(t => t.QueueSettingsName.Contains("DirectExchangeQueue-Settings")),
                            TopicExchange = RabbitMqSettingsProvider.Options.QueueSettings
                                .Single(t => t.QueueSettingsName.Contains("TopicExchangeQueue-Settings"))
                        };
                    }
                }
            }

            return _queues;
        }
    }
}
```

## Examples how to use in tests

```csharp
[TestFixture]
public class Tests 
{
    [Test]
    public void PublishMessageWithDefaultExchange_GetMessageFromQueue_ValidateMessage()
    {
        //Arrange
        var messageToRabbit = new SimpleClass();
        
        //Act
        TestServices.RabbitPublishers.FirstDefaultExchangeQueue.PublishMessage(messageToRabbit);

        var consumedRabbitMessage = TestServices.RabbitConsumers.FirstDefaultExchangeQueue.WaitAndGetMessages(
            x => x.Value.Id == messageToRabbit.Id).FirstOrDefault();

        //Assert
        consumedRabbitMessage.Should().NotBeNull();
        consumedRabbitMessage?.Value.Should().BeEquivalentTo(messageToRabbit);
    }
    
    [Test]
    public void PublishMessageWithDirectExchange_GetMessageFromQueue_ValidateMessage()
    {
        //Arrange
        var messageToRabbit1 = new SimpleClass();
        var messageToRabbit2 = new SimpleClass();
        
        //Act
        TestServices.RabbitPublishers.DirectExchangeQueue.PublishMessage(messageToRabbit1, "error");
        TestServices.RabbitPublishers.DirectExchangeQueue.PublishMessage(messageToRabbit2, "warning");
        
        var consumedRabbitMessage1 = TestServices.RabbitConsumers.DirectExchangeQueue.WaitAndGetMessages(
            x => x.Value.Id == messageToRabbit1.Id).FirstOrDefault();
        var consumedRabbitMessage2 = TestServices.RabbitConsumers.DirectExchangeQueue.WaitAndGetMessages(
            x => x.Value.Id == messageToRabbit2.Id).FirstOrDefault();
        
        //Assert
        consumedRabbitMessage1.Should().NotBeNull();
        consumedRabbitMessage1?.Value.Should().BeEquivalentTo(messageToRabbit1);
        consumedRabbitMessage2.Should().NotBeNull();
        consumedRabbitMessage2?.Value.Should().BeEquivalentTo(messageToRabbit2);
    }
    
    [Test]
    public void PublishMessageWithFanoutExchange_GetMessageFromQueue_ValidateMessage()
    {
        //Arrange
        var messageToRabbit = new SimpleClass();
        
        //Act
        TestServices.RabbitPublishers.FanoutExchangeQueue.PublishMessage(messageToRabbit);
        
        var consumedRabbitMessage = TestServices.RabbitConsumers.FanoutExchangeQueue.WaitAndGetMessages(
            x => x.Value.Id == messageToRabbit.Id).FirstOrDefault();
        
        //Assert
        consumedRabbitMessage.Should().NotBeNull();
        consumedRabbitMessage?.Value.Should().BeEquivalentTo(messageToRabbit);
    }
    
    [Test]
    public void PublishMessageWithTopicExchange_GetMessageFromQueue_ValidateMessage()
    {
        //Arrange
        var messageToRabbit = new SimpleClass();
        
        //Act
        TestServices.RabbitPublishers.TopicExchangeQueue.PublishMessage(
            messageToRabbit, "Tesla.red.fast.ecological");
        
        var consumedRabbitMessage = TestServices.RabbitConsumers.TopicExchangeQueue.WaitAndGetMessages(
            x => x.Value.Id == messageToRabbit.Id).FirstOrDefault();
        
        //Assert
        consumedRabbitMessage.Should().NotBeNull();
        consumedRabbitMessage?.Value.Should().BeEquivalentTo(messageToRabbit);
    }
}
```