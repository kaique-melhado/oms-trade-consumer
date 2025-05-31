using System.Xml.Linq;
using OmsTradeConsumer.Domain.Enums;
using OmsTradeConsumer.Domain.Models;

namespace OmsTradeConsumer.Application.Factories;

public static class XmlFactory
{
    public static XElement CreateXml(TradeModel tradeModel)
    {
        if (tradeModel == null) throw new ArgumentNullException(nameof(tradeModel));
        if (tradeModel.Product == null) throw new ArgumentNullException(nameof(tradeModel.Product));
        if (tradeModel.Order == null) throw new ArgumentNullException(nameof(tradeModel.Order));

        var clientType = (ClientType)tradeModel.Order.Client.ClientIdentificationType;
        var productType = (ProductType)tradeModel.Product.ProductType;

        return (clientType, productType) switch
        {
            (ClientType.Fundo, ProductType.RendaFixa) => new Generators.Fundos.RendaFixaGenerator().Generate(tradeModel),
            (ClientType.Fundo, ProductType.RendaVariavel) => new Generators.Fundos.RendaVariavelGenerator().Generate(tradeModel),
            (ClientType.Fundo, ProductType.CotasFundos) => new Generators.Fundos.CotasFundosGenerator().Generate(tradeModel),
            (ClientType.Fundo, ProductType.Derivativos) => new Generators.Fundos.DerivativosGenerator().Generate(tradeModel),
            (ClientType.Carteira, ProductType.RendaFixa) => new Generators.Carteiras.RendaFixaGenerator().Generate(tradeModel),
            (ClientType.Carteira, ProductType.RendaVariavel) => new Generators.Carteiras.RendaVariavelGenerator().Generate(tradeModel),
            (ClientType.Carteira, ProductType.CotasFundos) => new Generators.Carteiras.CotasFundosGenerator().Generate(tradeModel),
            (ClientType.Carteira, ProductType.Derivativos) => new Generators.Carteiras.DerivativosGenerator().Generate(tradeModel),
            _ => throw new InvalidOperationException("Invalid client or product type")
        };
    }
}