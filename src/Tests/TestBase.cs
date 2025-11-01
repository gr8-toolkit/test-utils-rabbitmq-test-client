using Ductus.FluentDocker.Services;
using Tests.Configuration;
using Tests.Configuration.DockerDependency;

namespace Tests;

[SetUpFixture]
public class AssemblySetUp
{
    private ICompositeService? Docker { get; set; }
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        //start docker with rabbitmq
        Docker = new DockerDependency().Start();
        
        //start all consumers and producers
        TestServices.RabbitPublishers.StartAll();
        TestServices.RabbitConsumers.StartAll();
    }

    [OneTimeTearDown]
    public void BaseTearDown()
    {
        //dispose all consumers and producers
        TestServices.RabbitConsumers.CloseAll();
        TestServices.RabbitPublishers.CloseAll();
        
        //dispose docker with rabbitmq
        Docker?.Dispose();
    }
}