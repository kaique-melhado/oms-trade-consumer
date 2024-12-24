using OmsTradeConsumer.Domain.Models;

namespace OmsTradeConsumer.Domain.Interfaces.Services;

public interface IFileTransferService
{
    Task SaveFileAsync(TradeModel tradeModel, string content);
}