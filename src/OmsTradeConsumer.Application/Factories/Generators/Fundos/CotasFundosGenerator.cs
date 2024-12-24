using OmsTradeConsumer.Domain.Models;
using System.Xml.Linq;

namespace OmsTradeConsumer.Application.Factories.Generators.Fundos;

public class CotasFundosGenerator : IXmlGenerator
{
    public XElement Generate(TradeModel tradeModel)
    {
        if (tradeModel == null) throw new ArgumentNullException(nameof(tradeModel));
        if (tradeModel.CustodyBankAccount == null) throw new ArgumentNullException(nameof(tradeModel.CustodyBankAccount));
        if (tradeModel.Product == null) throw new ArgumentNullException(nameof(tradeModel.Product));
        if (tradeModel.Order == null) throw new ArgumentNullException(nameof(tradeModel.Order));
        if (tradeModel.PortfolioHolder == null) throw new ArgumentNullException(nameof(tradeModel.PortfolioHolder));

        var template = new XElement("TemplateBoletaCotasFundosUpload",
            new XElement("Acao", "INCLUIR"),
            new XElement("ContaMovimentoOrigem", tradeModel.CustodyBankAccount.ExternalId),
            tradeModel.Order.MovementTypeDescription != "Aplicação" ?
                new XElement("ContaMovimentoDestino", tradeModel.CustodyBankAccount.ExternalId) : null,
            new XElement("DataBoleta", tradeModel.MovementDate.ToString("yyyy-MM-dd")),
            new XElement("DataConversao", tradeModel.QuotationDate.ToString("yyyy-MM-dd")),
            new XElement("DataLiquidacao", tradeModel.SettlementDate.ToString("yyyy-MM-dd")),
            new XElement("DataRegistro", tradeModel.MovementDate.ToString("yyyy-MM-dd")),
            new XElement("EntradaFinanceiraBradesco", tradeModel.CustodyBankAccount.ExternalId),
            tradeModel.Order.MovementTypeDescription == "Aplicação" ?
                new XElement("OrigemAplicacao", tradeModel.TradeReason) : null,
            tradeModel.Order.MovementTypeDescription != "Aplicação" ?
                new XElement("MotivoResgate", tradeModel.TradeReason) : null,
            new XElement("Produto", tradeModel.Product.ExternalProductId),
            tradeModel.Price.HasValue ? new XElement("PU", tradeModel.Price.Value) : null,
            tradeModel.Quantity.HasValue ? new XElement("Quantidade", tradeModel.Quantity.Value) : null,
            new XElement("SomenteInformacao", tradeModel.IsForInformationOnly ? "Sim" : "Não"),
            new XElement("TipoBoleta", "Cotas de Fundos"),
            tradeModel.Order.MovementTypeDescription != "Aplicação" ?
                new XElement("TipoContaDestino", "Cadastradas na Conta Movimento Origem") : null,
            new XElement("TipoMovimento", tradeModel.Order.MovementTypeDescription),
            new XElement("Titularidade", tradeModel.PortfolioHolder.ExternalId),
            new XElement("ValorBrutoOperacao", tradeModel.FinancialValue),
            new XElement("ValorLiquidoOperacao", tradeModel.FinancialValue)
        );

        var boletaCotasFundos = new XElement("BoletaCotasFundos", template);
        var boletaXml = new XElement("BoletaXML",
            new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
            new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"),
            boletaCotasFundos
        );

        return boletaXml;
    }
}
