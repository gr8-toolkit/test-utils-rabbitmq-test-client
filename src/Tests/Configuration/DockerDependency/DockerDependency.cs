using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Services;

namespace Tests.Configuration.DockerDependency;

public class DockerDependency : IDisposable
{
    private readonly ICompositeService _dockerCompose;

    public DockerDependency()
    {
        //TODO Find why it's not fully waiting for docker compose start
        /*var file = Path.Combine(
            Directory.GetCurrentDirectory(),
            (TemplateString)"docker-compose.yml");

        _dockerCompose = new Builder()
            .UseContainer()
            .UseCompose()
            .FromFile(file)
            .RemoveOrphans()
            .Build();*/
        
        _dockerCompose = new Builder()
            .UseContainer()
            .UseImage("rabbitmq:3.11.8-management")
            .WithHostName("rabbitmq")
            .WithName("rabbitmq")
            .ExposePort(15672, 15672)
            .ExposePort(5672, 5672)
            .WaitForMessageInLog("Server startup complete", TimeSpan.FromSeconds(15))
            .Builder()
            .Build();
    }

    public ICompositeService Start()
    {
        return _dockerCompose.Start();
    }
    
    public void Dispose()
    {
        _dockerCompose.Dispose();
    }
}