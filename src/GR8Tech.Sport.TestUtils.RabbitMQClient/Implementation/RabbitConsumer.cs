using System.Collections.Concurrent;
using System.Text;
using GR8Tech.Sport.TestUtils.RabbitMQClient.Configurations;
using GR8Tech.Sport.TestUtils.RabbitMQClient.Configurations.Options;
using GR8Tech.Sport.TestUtils.RabbitMQClient.Connection;
using GR8Tech.Sport.TestUtils.RabbitMQClient.Contracts;
using GR8Tech.Sport.TestUtils.RabbitMQClient.Helpers;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Headers = GR8Tech.Sport.TestUtils.RabbitMQClient.Helpers.Headers;

namespace GR8Tech.Sport.TestUtils.RabbitMQClient.Implementation;

public class RabbitConsumer<TValue> : IRabbitConsumer<TValue>
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly object _lockObject = new();
    private readonly QueueSettings _queueSettings;
    private ConcurrentQueue<DataHolder<TValue>> _consumedMessages;
    private readonly AsyncEventingBasicConsumer _consumer;
    private readonly ILogger _logger;

    public RabbitConsumer(QueueSettings settings)
    {
        #region SetUp Queue

        _consumedMessages = new ConcurrentQueue<DataHolder<TValue>>();
        _connection = RabbitMQConnection.GetInstance().Connection;
        _queueSettings = settings;
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(_queueSettings.Queue,
            _queueSettings.Durable,
            _queueSettings.Exclusive,
            _queueSettings.AutoDelete,
            _queueSettings.Arguments);
        _logger = RabbitMqSettingsProvider.Logger.ForContext<RabbitConsumer<TValue>>();

        #endregion

        #region SetUp Exchange

        switch (_queueSettings.ExchangeType.ToLower())
        {
            case "fanout":
                _channel.ExchangeDeclare(exchange: _queueSettings.ExchangeName, type: ExchangeType.Fanout);
                _channel.QueueBind(queue: _queueSettings.Queue, exchange: _queueSettings.ExchangeName, routingKey: string.Empty);
                break;
            case "direct":
                _channel.ExchangeDeclare(exchange: _queueSettings.ExchangeName, type: ExchangeType.Direct);
                foreach (var bindingSettings in _queueSettings.Bindings)
                    _channel.QueueBind(queue: _queueSettings.Queue, exchange: _queueSettings.ExchangeName, routingKey: bindingSettings.RoutingKey);
                break;
            case "topic":
                _channel.ExchangeDeclare(exchange: _queueSettings.ExchangeName, type: ExchangeType.Topic);
                _channel.QueueBind(queue: _queueSettings.Queue, exchange: _queueSettings.ExchangeName, routingKey: _queueSettings.RoutingKey);
                break;
        }

        #endregion

        #region SetUpConsumer

        _consumer = new AsyncEventingBasicConsumer(_channel);
        
        SetUpMessageProcessing();

        #endregion
    }

    public void StartConsume()
    {
        try 
        { 
            _channel.BasicConsume(
                _queueSettings.Queue, 
                _queueSettings.AutoAck, 
                _consumer);
        }
        catch (Exception e)
        {
            _logger.Warning("Consume error: {errorMessage}", e);
        }
    }

    public List<DataHolder<TValue>> GetMessages(Func<DataHolder<TValue>, bool> compare)
    {
        var consumedMessagesList = _consumedMessages.ToList();

        return consumedMessagesList?.FindAll(m => compare(m));
    }

    public List<DataHolder<TValue>> WaitAndGetMessages(Func<DataHolder<TValue>, bool> compare)
    {
        WaitForMessage(compare);

        var consumedMessagesList = _consumedMessages?.ToList();

        return consumedMessagesList?.FindAll(m => compare(m));
    }

    public List<DataHolder<TValue>> WaitAndGetMessages(Func<DataHolder<TValue>, bool> compare,
        int timeoutRetries, int delaysBetweenRetries)
    {
        WaitForMessage(compare, timeoutRetries, delaysBetweenRetries);

        var consumedMessagesList = _consumedMessages?.ToList();

        return consumedMessagesList?.FindAll(m => compare(m));
    }

    public void WaitForMessage(Func<DataHolder<TValue>, bool> compare, int timeoutRetries,
        int delaysBetweenRetries)
    {
        var policy = PolicyProvider(timeoutRetries, delaysBetweenRetries);

        policy.Execute(() =>
        {
            if (_consumedMessages == null) throw new Exception("_consumedMessages is null");
            if (_consumedMessages.Count == 0)
                throw new Exception("No messages in queue found: " + _queueSettings.Queue);

            if (_consumedMessages.All(x => !compare(x)))
                throw new Exception(
                    $"Entities are different. Among messages no matches in queue: {_queueSettings.Queue}. " +
                    $"Count: {_consumedMessages.Count}");
        });
    }

    public void WaitForMessage(Func<DataHolder<TValue>, bool> compare)
    {
        var timeoutRetries = RabbitMqSettingsProvider.Options.PollingSettings.DefaultTimeoutRetries;
        var delaysBetweenRetries = RabbitMqSettingsProvider.Options.PollingSettings.DefaultDelaysBetweenRetries;

        WaitForMessage(compare, timeoutRetries, delaysBetweenRetries);
    }

    public List<DataHolder<TValue>> WaitAndCheckThatMsgIsNotPresent(Func<DataHolder<TValue>, bool> compare)
    {
        CheckThatMsgIsNotPresent(compare);

        var consumedMessagesList = _consumedMessages?.ToList();

        return consumedMessagesList?.FindAll(m => compare(m));
    }

    public List<DataHolder<TValue>> WaitAndCheckThatMsgIsNotPresent(
        Func<DataHolder<TValue>, bool> compare,
        int timeoutRetries, int delaysBetweenRetries)
    {
        CheckThatMsgIsNotPresent(compare, timeoutRetries, delaysBetweenRetries);

        var consumedMessagesList = _consumedMessages?.ToList();

        return consumedMessagesList?.FindAll(m => compare(m));
    }

    public void CheckThatMsgIsNotPresent(Func<DataHolder<TValue>, bool> compare)
    {
        var timeoutRetries = RabbitMqSettingsProvider.Options.PollingSettings.DefaultTimeoutRetries;
        var delaysBetweenRetries = RabbitMqSettingsProvider.Options.PollingSettings.DefaultDelaysBetweenRetries;

        CheckThatMsgIsNotPresent(compare, timeoutRetries, delaysBetweenRetries);
    }

    public void CheckThatMsgIsNotPresent(Func<DataHolder<TValue>, bool> compare, int timeoutRetries,
        int delaysBetweenRetries)
    {
        var result = PolicyProvider(timeoutRetries, delaysBetweenRetries).ExecuteAndCapture(() =>
        {
            var msg = _consumedMessages.Where(compare).Select(m => m).ToList();

            if (msg.Count == 0)
                throw new Exception("There is no messages matching to the compare." +
                                    $"\nWaited for object to be present - {timeoutRetries * delaysBetweenRetries} ms." +
                                    $"\nRe-checked Received messages {timeoutRetries} times."
                                    + _queueSettings.Queue);

            return msg;
        });

        if (result.Result == null) _logger.Warning("CheckThatMsgIsNotPresent result.Result == null. {result.FinalException.Message}", result.FinalException.Message);

        if (result.Result != null && result.Result.Count > 0)
            throw new Exception($"{result.GetType().Name} was present in received massages");
    }

    public void ForceFullCleanMessagesCollection()
    {
        lock (_lockObject)
        {
            _consumedMessages.Clear();
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }

    #region Private methods
    
    private void SetUpMessageProcessing()
    {
        if (_consumer is null)
            throw new NullReferenceException("Consumer should be initialized in class constructor");

        try
        {
            _consumer.Received += (ch, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.Span);
                var props = ea?.BasicProperties;
                var propKey = props?.Headers?.GetValue("Key");
                var newMessage = new DataHolder<TValue>();

                if (message.IsMessageCanBeDeserialized())
                {
                    var newMessageValue = message.GetDeserializedObject<TValue>();
                    newMessage.Value = newMessageValue;
                }

                if (propKey != string.Empty || propKey != null)
                {
                    newMessage.Headers = new Headers();
                    newMessage.Headers.Key = propKey;
                }

                if (newMessage is not null)
                {
                    lock (_lockObject)
                    {
                        _consumedMessages.Enqueue(newMessage);
                    }
                }

                if (RabbitMqSettingsProvider.Options.CleanUpQueueSettings.IsCleanUpEnabled &&
                    _consumedMessages.Count > RabbitMqSettingsProvider.Options.CleanUpQueueSettings.FromMessageCount)
                {
                    lock (_lockObject)
                    {
                        _consumedMessages.TryDequeue(out _);
                    }
                }

                return Task.CompletedTask;
            };
        }
        catch (Exception e)
        {
            _logger.Warning("Consume error: {@errorMessage}", e);
        }
    }

    private static RetryPolicy PolicyProvider(int numberOfRetries, int delayBetweenRetriesSeconds)
    {
        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetry(numberOfRetries, retryAttempt => TimeSpan.FromSeconds(delayBetweenRetriesSeconds));

        return policy;
    }

    #endregion
}