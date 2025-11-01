using GR8Tech.Sport.TestUtils.RabbitMQClient.Helpers;

namespace GR8Tech.Sport.TestUtils.RabbitMQClient.Contracts;

public interface IRabbitConsumer<TValue> : IRabbitConsume, IDisposable
{
    List<DataHolder<TValue>> GetMessages(Func<DataHolder<TValue>, bool> compare);
    List<DataHolder<TValue>> WaitAndGetMessages(Func<DataHolder<TValue>, bool> compare);
    List<DataHolder<TValue>> WaitAndGetMessages(Func<DataHolder<TValue>, bool> compare, int timeoutRetries, int delaysBetweenRetries);
    void WaitForMessage(Func<DataHolder<TValue>, bool> compare, int timeoutRetries, int delaysBetweenRetries);
    void WaitForMessage(Func<DataHolder<TValue>, bool> compare);
    List<DataHolder<TValue>> WaitAndCheckThatMsgIsNotPresent(Func<DataHolder<TValue>, bool> compare);
    List<DataHolder<TValue>> WaitAndCheckThatMsgIsNotPresent(Func<DataHolder<TValue>, bool> compare, int timeoutRetries, int delaysBetweenRetries);
    void CheckThatMsgIsNotPresent(Func<DataHolder<TValue>, bool> compare);
    void CheckThatMsgIsNotPresent(Func<DataHolder<TValue>, bool> compare, int timeoutRetries, int delaysBetweenRetries);
    void ForceFullCleanMessagesCollection();
}