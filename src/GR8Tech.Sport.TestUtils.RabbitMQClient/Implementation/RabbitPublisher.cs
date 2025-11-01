using System.Text;
using System.Text.Json;
using GR8Tech.Sport.TestUtils.RabbitMQClient.Configurations;
using GR8Tech.Sport.TestUtils.RabbitMQClient.Configurations.Options;
using GR8Tech.Sport.TestUtils.RabbitMQClient.Connection;
using GR8Tech.Sport.TestUtils.RabbitMQClient.Contracts;
using RabbitMQ.Client;
using Serilog;

namespace GR8Tech.Sport.TestUtils.RabbitMQClient.Implementation;

public class RabbitPublisher<TValue> : IRabbitPublisher<TValue>
{
    private IConnection _connection;
    private readonly IModel _channel;
    private readonly QueueSettings _queueSettings;
    private readonly ILogger _logger;

    public RabbitPublisher(QueueSettings settings)
    {
        _connection = RabbitMQConnection.GetInstance().Connection;
        _queueSettings = settings;
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: _queueSettings.Queue,
            durable: _queueSettings.Durable,
            exclusive: _queueSettings.Exclusive,
            autoDelete: _queueSettings.AutoDelete,
            arguments: _queueSettings.Arguments);
        
        _logger = RabbitMqSettingsProvider.Logger.ForContext<RabbitPublisher<TValue>>();
        
        switch (_queueSettings.ExchangeType.ToLower())
        {
            case "fanout":
                _channel.ExchangeDeclare(exchange: _queueSettings.ExchangeName, type: ExchangeType.Fanout);
                break;
            case "direct":
                _channel.ExchangeDeclare(exchange: _queueSettings.ExchangeName, type: ExchangeType.Direct);
                break;
            case "topic":
                _channel.ExchangeDeclare(exchange: _queueSettings.ExchangeName, type: ExchangeType.Topic);
                break;
        }
    }
    
    public void PublishMessage(TValue message, string? routingKey = null)
    {
        try
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            _channel.BasicPublish(exchange: _queueSettings.ExchangeName,
                routingKey: routingKey ?? _queueSettings.RoutingKey,
                basicProperties: null,
                body: body);
        
            _logger.Debug("Sent message to Rabbit: {@message}", message);
        }
        catch (Exception e)
        {
            _logger.Error("Publishing failed for message: {@message}. And exeption {@exception}", message, e);
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}