namespace GR8Tech.Sport.TestUtils.RabbitMQClient.Configurations.Options;

public class EnvironmentSettings
{
    public string HostName { get; set; }

    public string Env { get; set; }

    public EnvironmentSettings(string hostName, string env)
    {
        HostName = hostName;
        Env = env;
    }
}