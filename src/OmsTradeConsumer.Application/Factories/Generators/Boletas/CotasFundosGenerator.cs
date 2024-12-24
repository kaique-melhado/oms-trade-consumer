using OmsTradeConsumer.Domain.Models;
using System.Xml.Linq;

namespace OmsTradeConsumer.Application.Factories.Generators.Boletas;

public class CotasFundosGenerator : IXmlGenerator
{
    public XElement Generate(TradeModel tradeModel)
    {
        if (tradeModel == null) throw new ArgumentNullException(nameof(tradeModel));
        if (tradeModel.CustodyBankAccount == null) throw new ArgumentNullException(nameof(tradeModel.CustodyBankAccount));
        if (tradeModel.Product == null) throw new ArgumentNullException(nameof(tradeModel.Product));
        if (tradeModel.Order == null) throw new ArgumentNullException(nameof(tradeModel.Order));
        if (tradeModel.PortfolioHolder == null) throw new ArgumentNullException(nameof(tradeModel.PortfolioHolder));

        var tipoMovimento = tradeModel.Order.MovementTypeDescription;

        var template = new XElement("TemplateBoletaCotasFundosUpload",
            new XElement("Acao", "INCLUIR"),
            new XElement("ContaMovimentoOrigem", tradeModel.CustodyBankAccount.ExternalId),
            tipoMovimento != "Aplicação"
                ? new XElement("ContaMovimentoDestino", tradeModel.SettlementBankAccount.ExternalId)
                : null,
            new XElement("Contraparte", tradeModel.Counterparty.CounterpartyId),
            new XElement("DataBoleta", tradeModel.MovementDate),
            new XElement("DataConversao", tradeModel.QuotationDate),
            new XElement("DataLiquidacao", tradeModel.SettlementDate),
            new XElement("DataRegistro", tradeModel.MovementDate),
            tipoMovimento == "Aplicação"
                ? new XElement("EntradaFinanceiraBradesco", tradeModel.SettlementBankAccount.ExternalId)
                : null,
            tipoMovimento == "Aplicação"
                ? new XElement("OrigemAplicacao", tradeModel.TradeReason)
                : null,
            tipoMovimento != "Aplicação"
                ? new XElement("MotivoResgate", tradeModel.TradeReason)
                : null,
            new XElement("Produto", tradeModel.Product.ExternalProductId),
            tradeModel.Price.HasValue
                ? new XElement("PU", tradeModel.Price)
                : null,
            tradeModel.Quantity.HasValue
                ? new XElement("Quantidade", tradeModel.Quantity)
                : null,
            new XElement("SomenteInformacao", tradeModel.IsForInformationOnly ? "Sim" : "Não"),
            new XElement("TipoBoleta", "Cotas de Fundos"),
            tipoMovimento != "Aplicação"
                ? new XElement("TipoContaDestino", "Cadastradas na Conta Movimento Origem")
                : null,
            new XElement("TipoMovimento", tipoMovimento),
            new XElement("Titularidade", tradeModel.PortfolioHolder.ExternalId),
            new XElement("ValorBrutoOperacao", tradeModel.FinancialValue),
            new XElement("ValorLiquidoOperacao", tradeModel.FinancialValue)
        );

        return new XElement("BoletaCotasFundos", template);
    }
}