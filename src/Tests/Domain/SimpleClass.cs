namespace Tests.Domain;

public class SimpleClass
{
    public int Id { get; set; } = new Random().Next(int.MinValue, int.MaxValue);
    public string? Information { get; set; } = Guid.NewGuid().ToString();
}