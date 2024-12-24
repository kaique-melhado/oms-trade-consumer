using System.Xml.Linq;
using OmsTradeConsumer.Domain.Models;

namespace OmsTradeConsumer.Application.Factories;

public interface IXmlGenerator
{
    XElement Generate(TradeModel tradeModel);
}
