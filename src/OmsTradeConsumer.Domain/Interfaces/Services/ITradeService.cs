using OmsTradeConsumer.Domain.Models;

namespace OmsTradeConsumer.Domain.Interfaces.Services;

public interface ITradeService
{
    Task ProcessTradeAsync(TradeModel tradeModel);
}