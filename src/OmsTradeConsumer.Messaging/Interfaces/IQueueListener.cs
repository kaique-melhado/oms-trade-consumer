namespace OmsTradeConsumer.Messaging.Interfaces;

public interface IQueueListener
{
    Task ListenForMessagesAsync();
    Task ProcessMessageAsync(string message);
}