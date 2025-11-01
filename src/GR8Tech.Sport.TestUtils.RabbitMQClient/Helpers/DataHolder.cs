namespace GR8Tech.Sport.TestUtils.RabbitMQClient.Helpers;

public class DataHolder<T>
{
    public Headers Headers { get; set; }
    public T Value { get; set; }
}

public class Headers
{
    public string Key { get; set; }
}