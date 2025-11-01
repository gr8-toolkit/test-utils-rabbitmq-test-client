namespace GR8Tech.Sport.TestUtils.RabbitMQClient.Configurations.Options;

public class PollingSettings
{
    public int DefaultTimeoutRetries { get; set; }

    public int DefaultDelaysBetweenRetries { get; set; }

    public PollingSettings(int defaultTimeoutRetries, int defaultDelaysBetweenRetries)
    {
        DefaultTimeoutRetries = defaultTimeoutRetries;
        DefaultDelaysBetweenRetries = defaultDelaysBetweenRetries;
    }
}