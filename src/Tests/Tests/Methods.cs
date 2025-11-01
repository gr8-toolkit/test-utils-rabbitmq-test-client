using FluentAssertions;
using Tests.Configuration;
using Tests.Domain;

namespace Tests.Tests;

[TestFixture]
public class Methods 
{
    [Test]
    public void MessageIsNotSend_CheckThatMsgIsNotPresent_MessageDoesNotExist()
    {
        //Arrange
        var messageToRabbit = new SimpleClass();
        
        //Act

        //Assert
        TestServices.RabbitConsumers.SecondDefaultExchangeQueue.CheckThatMsgIsNotPresent(x =>
            x.Value.Id == messageToRabbit.Id);
    }
    
    [Test]
    public void MessageIsNotSend_WaitAndCheckThatMsgIsNotPresent_CollectionWithMessagesIsEmpty()
    {
        //Arrange
        var messageToRabbit = new SimpleClass();
        
        //Act
        TestServices.RabbitPublishers.SecondDefaultExchangeQueue.PublishMessage(messageToRabbit);
        
        //Assert
        var consumedRabbitMessage =
            TestServices.RabbitConsumers.SecondDefaultExchangeQueue.WaitAndCheckThatMsgIsNotPresent(x =>
                x.Value.Id == messageToRabbit.Id && x.Value.Information == "InvalidMessage");

        consumedRabbitMessage.Should().NotBeNull();
        consumedRabbitMessage.Should().BeEmpty();
    }
    
    [Test]
    public void PublishMessage_WaitAndGetMessages_ValidateMessage()
    {
        //Arrange
        var messageToRabbit = new SimpleClass();
        
        //Act
        TestServices.RabbitPublishers.SecondDefaultExchangeQueue.PublishMessage(messageToRabbit);

        //Assert
        var consumedRabbitMessage = TestServices.RabbitConsumers.SecondDefaultExchangeQueue.WaitAndGetMessages(x =>
            x.Value.Id == messageToRabbit.Id).FirstOrDefault();
        
        consumedRabbitMessage.Should().NotBeNull();
        consumedRabbitMessage?.Value.Should().BeEquivalentTo(messageToRabbit);
    }
    
    [Test]
    public void PublishMessage_WaitForMessage_MessageExists()
    {
        //Arrange
        var messageToRabbit = new SimpleClass();
        
        //Act
        TestServices.RabbitPublishers.SecondDefaultExchangeQueue.PublishMessage(messageToRabbit);

        //Assert
        TestServices.RabbitConsumers.SecondDefaultExchangeQueue.WaitForMessage(x =>
            x.Value.Id == messageToRabbit.Id);
    }
    
    [Test]
    public void PublishMessage_ForceFullCleanMessagesCollection_MessageDoesNotExist()
    {
        //Arrange
        var messageToRabbit = new SimpleClass();
        
        //Act
        TestServices.RabbitPublishers.ThirdDefaultExchangeQueue.PublishMessage(messageToRabbit);

        //Assert
        TestServices.RabbitConsumers.ThirdDefaultExchangeQueue.WaitForMessage(x =>
            x.Value.Id == messageToRabbit.Id);

        TestServices.RabbitConsumers.ThirdDefaultExchangeQueue.ForceFullCleanMessagesCollection();
        
        TestServices.RabbitConsumers.ThirdDefaultExchangeQueue.WaitAndCheckThatMsgIsNotPresent(x =>
            x.Value.Id == messageToRabbit.Id);
    }
}