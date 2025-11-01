using GR8Tech.Sport.TestUtils.RabbitMQClient.Configurations;
using RabbitMQ.Client;

namespace GR8Tech.Sport.TestUtils.RabbitMQClient.Connection;

internal sealed class RabbitMQConnection
{
    private static object syncRoot = new();
    private static RabbitMQConnection _rabbitMQConnection;
    public IConnection Connection { get; private set; }

    private RabbitMQConnection()
    {
        var factory = new ConnectionFactory { HostName = RabbitMqSettingsProvider.Options.EnvironmentSettings.HostName };
        factory.DispatchConsumersAsync = true;
        Connection = factory.CreateConnection();
    }
    
    public static RabbitMQConnection GetInstance()
    {
        if (_rabbitMQConnection is null)
        {
            lock (syncRoot)
            {
                if (_rabbitMQConnection == null)
                    _rabbitMQConnection = new RabbitMQConnection();
            }
        }
        return _rabbitMQConnection;
    }
}