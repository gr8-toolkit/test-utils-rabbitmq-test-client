using GR8Tech.Sport.TestUtils.RabbitMQClient.Configurations;
using GR8Tech.Sport.TestUtils.RabbitMQClient.Configurations.Options;

namespace Tests.Configuration.RabbitMQ;

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