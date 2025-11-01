using GR8Tech.Sport.TestUtils.RabbitMQClient.Configurations.Options;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace GR8Tech.Sport.TestUtils.RabbitMQClient.Configurations;

public static class RabbitMqSettingsProvider
{
    public static RabbitMqConfiguration Options { get; }
    internal static ILogger Logger { get; }
    
    static RabbitMqSettingsProvider()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("test-settings.json")
            .AddJsonFile($"test-settings.{Environment.MachineName}.json", true)
            .AddEnvironmentVariables()
            .Build();
        
        Options = config
            .GetSection("RabbitMqConfiguration")
            .Get<RabbitMqConfiguration>()!;

        Options.PollingSettings ??= new PollingSettings(30, 1);
        
        Options.CleanUpQueueSettings ??= new CleanUpQueueSettings(true, 200);
        
        Options.EnvironmentSettings ??= new EnvironmentSettings("localhost", "local");
        
        Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .WriteTo.Async(x => x.Console(theme:AnsiConsoleTheme.Code), 10)
            .CreateLogger();
        
        ApplyEnvVariableToTopicsName();
        
        Logger.ForContext("SourceContext", typeof(RabbitMqSettingsProvider))
            .Information("RabbitMQSettings have been read and initialized");
    }
    
    
    private static void ApplyEnvVariableToTopicsName()
    {
        foreach (var queueSettings in Options.QueueSettings)
        {
            if (queueSettings.Queue.Contains("${env}"))
            {
                queueSettings.Queue =
                    queueSettings.Queue.Replace("${env}", Options.EnvironmentSettings.Env);
            }
            if (queueSettings.RoutingKey.Contains("${env}"))
            {
                queueSettings.RoutingKey =
                    queueSettings.RoutingKey.Replace("${env}", Options.EnvironmentSettings.Env);
            }
            if (queueSettings.Bindings is not null)
            {
                foreach (var binding in queueSettings.Bindings)
                {
                    binding.RoutingKey =
                        binding.RoutingKey.Replace("${env}", Options.EnvironmentSettings.Env);
                }
            }
        }
    }
}
