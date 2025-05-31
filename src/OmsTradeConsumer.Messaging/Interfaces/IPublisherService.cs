namespace OmsTradeConsumer.Messaging.Interfaces;

public interface IPublisherService
{
    Task SendMessage(string message);
}
