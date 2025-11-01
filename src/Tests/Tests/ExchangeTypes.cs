using FluentAssertions;
using Tests.Configuration;
using Tests.Domain;

namespace Tests.Tests;

[TestFixture]
public class ExchangeTypes 
{
    [Test]
    public void PublishMessageWithDefaultExchange_GetMessageFromQueue_ValidateMessage()
    {
        //Arrange
        var messageToRabbit = new SimpleClass();
        
        //Act
        TestServices.RabbitPublishers.FirstDefaultExchangeQueue.PublishMessage(messageToRabbit);

        var consumedRabbitMessage = TestServices.RabbitConsumers.FirstDefaultExchangeQueue.WaitAndGetMessages(
            x => x.Value.Id == messageToRabbit.Id).FirstOrDefault();

        //Assert
        consumedRabbitMessage.Should().NotBeNull();
        consumedRabbitMessage?.Value.Should().BeEquivalentTo(messageToRabbit);
    }
    
    [Test]
    public void PublishMessageWithDirectExchange_GetMessageFromQueue_ValidateMessage()
    {
        //Arrange
        var messageToRabbit1 = new SimpleClass();
        var messageToRabbit2 = new SimpleClass();
        
        //Act
        TestServices.RabbitPublishers.DirectExchangeQueue.PublishMessage(messageToRabbit1, "error");
        TestServices.RabbitPublishers.DirectExchangeQueue.PublishMessage(messageToRabbit2, "warning");
        
        var consumedRabbitMessage1 = TestServices.RabbitConsumers.DirectExchangeQueue.WaitAndGetMessages(
            x => x.Value.Id == messageToRabbit1.Id).FirstOrDefault();
        var consumedRabbitMessage2 = TestServices.RabbitConsumers.DirectExchangeQueue.WaitAndGetMessages(
            x => x.Value.Id == messageToRabbit2.Id).FirstOrDefault();
        
        //Assert
        consumedRabbitMessage1.Should().NotBeNull();
        consumedRabbitMessage1?.Value.Should().BeEquivalentTo(messageToRabbit1);
        consumedRabbitMessage2.Should().NotBeNull();
        consumedRabbitMessage2?.Value.Should().BeEquivalentTo(messageToRabbit2);
    }
    
    [Test]
    public void PublishMessageWithFanoutExchange_GetMessageFromQueue_ValidateMessage()
    {
        //Arrange
        var messageToRabbit = new SimpleClass();
        
        //Act
        TestServices.RabbitPublishers.FanoutExchangeQueue.PublishMessage(messageToRabbit);
        
        var consumedRabbitMessage = TestServices.RabbitConsumers.FanoutExchangeQueue.WaitAndGetMessages(
            x => x.Value.Id == messageToRabbit.Id).FirstOrDefault();
        
        //Assert
        consumedRabbitMessage.Should().NotBeNull();
        consumedRabbitMessage?.Value.Should().BeEquivalentTo(messageToRabbit);
    }
    
    [Test]
    public void PublishMessageWithTopicExchange_GetMessageFromQueue_ValidateMessage()
    {
        //Arrange
        var messageToRabbit = new SimpleClass();
        
        //Act
        TestServices.RabbitPublishers.TopicExchangeQueue.PublishMessage(
            messageToRabbit, "Tesla.red.fast.ecological");
        
        var consumedRabbitMessage = TestServices.RabbitConsumers.TopicExchangeQueue.WaitAndGetMessages(
            x => x.Value.Id == messageToRabbit.Id).FirstOrDefault();
        
        //Assert
        consumedRabbitMessage.Should().NotBeNull();
        consumedRabbitMessage?.Value.Should().BeEquivalentTo(messageToRabbit);
    }
}