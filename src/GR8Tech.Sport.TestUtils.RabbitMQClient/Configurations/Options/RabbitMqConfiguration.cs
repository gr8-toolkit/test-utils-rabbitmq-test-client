namespace GR8Tech.Sport.TestUtils.RabbitMQClient.Configurations.Options;

public class RabbitMqConfiguration
{
    public QueueSettings[]? QueueSettings { get; set; }
    internal EnvironmentSettings? EnvironmentSettings { get; set; }
    internal CleanUpQueueSettings? CleanUpQueueSettings { get; set; }
    internal PollingSettings? PollingSettings { get; set; }
}