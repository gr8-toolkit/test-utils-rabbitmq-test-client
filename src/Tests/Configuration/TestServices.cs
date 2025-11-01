using Tests.Configuration.RabbitMQ;

namespace Tests.Configuration;

public static class TestServices
{
    public static IRabbitConsumers RabbitConsumers { get; }
    public static IRabbitPublishers RabbitPublishers { get; }

    static TestServices()
    {
        RabbitConsumers = new RabbitConsumers();
        RabbitPublishers = new RabbitPublishers();
    }
}