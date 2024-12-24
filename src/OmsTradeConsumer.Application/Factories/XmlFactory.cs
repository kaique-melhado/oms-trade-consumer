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
            (ClientType.CotasDeFundo, ProductType.RendaFixa) => new Generators.Fundos.RendaFixaGenerator().Generate(tradeModel),
            (ClientType.CotasDeFundo, ProductType.RendaVariavel) => new Generators.Fundos.RendaVariavelGenerator().Generate(tradeModel),
            (ClientType.CotasDeFundo, ProductType.CotasFundos) => new Generators.Fundos.CotasFundosGenerator().Generate(tradeModel),
            (ClientType.CotasDeFundo, ProductType.Derivativos) => new Generators.Fundos.DerivativosGenerator().Generate(tradeModel),
            (ClientType.NaoCotasDeFundo, ProductType.RendaFixa) => new Generators.Boletas.RendaFixaGenerator().Generate(tradeModel),
            (ClientType.NaoCotasDeFundo, ProductType.RendaVariavel) => new Generators.Boletas.RendaVariavelGenerator().Generate(tradeModel),
            (ClientType.NaoCotasDeFundo, ProductType.CotasFundos) => new Generators.Boletas.CotasFundosGenerator().Generate(tradeModel),
            (ClientType.NaoCotasDeFundo, ProductType.Derivativos) => new Generators.Boletas.DerivativosGenerator().Generate(tradeModel),
            _ => throw new NotImplementedException("Invalid client or product type")
        };
    }
}