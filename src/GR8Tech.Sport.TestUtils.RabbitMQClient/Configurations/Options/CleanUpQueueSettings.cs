namespace GR8Tech.Sport.TestUtils.RabbitMQClient.Configurations.Options;

public class CleanUpQueueSettings
{
    public bool IsCleanUpEnabled { get; set; }
    public int FromMessageCount { get; set; }
    
    public CleanUpQueueSettings(bool isCleanUpEnabled, int fromMessageCount)
    {
        IsCleanUpEnabled = isCleanUpEnabled;
        FromMessageCount = fromMessageCount;
    }
}