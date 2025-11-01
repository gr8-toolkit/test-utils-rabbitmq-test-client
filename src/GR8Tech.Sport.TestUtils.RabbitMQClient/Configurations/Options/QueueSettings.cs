namespace GR8Tech.Sport.TestUtils.RabbitMQClient.Configurations.Options;

public class QueueSettings
{
    public string QueueSettingsName { get; set; }
    public string Queue { get; set; }
    public string ExchangeType { get; set; }
    public string ExchangeName { get; set; }
    public string RoutingKey { get; set; }
    public bool Durable { get; set; } 
    public bool Exclusive { get; set; }
    public bool AutoDelete { get; set; }
    public bool AutoAck { get; set; }
    public IDictionary<string, object> Arguments { get; set; }
    public Binding[] Bindings { get; set; }
}

public class Binding
{
    public string RoutingKey { get; set; }
}