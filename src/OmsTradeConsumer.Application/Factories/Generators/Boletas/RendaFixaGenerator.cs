using OmsTradeConsumer.Domain.Models;
using System.Xml.Linq;

namespace OmsTradeConsumer.Application.Factories.Generators.Boletas;

public class RendaFixaGenerator : IXmlGenerator
{
    public XElement Generate(TradeModel tradeModel)
    {
        if (tradeModel == null) throw new ArgumentNullException(nameof(tradeModel));
        if (tradeModel.CustodyBankAccount == null) throw new ArgumentNullException(nameof(tradeModel.CustodyBankAccount));
        if (tradeModel.Product == null) throw new ArgumentNullException(nameof(tradeModel.Product));
        if (tradeModel.Order == null) throw new ArgumentNullException(nameof(tradeModel.Order));
        if (tradeModel.PortfolioHolder == null) throw new ArgumentNullException(nameof(tradeModel.PortfolioHolder));

        var tipoMovimento = tradeModel.Order.MovementTypeDescription;

        var template = new XElement("TemplateBoletaRendaFixaUpload",
            new XElement("Acao", "INCLUIR"),
            new XElement("ContaMovimentoOrigem", tradeModel.CustodyBankAccount.ExternalId),
            tipoMovimento != "Compra"
                ? new XElement("ContaMovimentoDestino", tradeModel.SettlementBankAccount.ExternalId)
                : null,
            new XElement("Contraparte", tradeModel.Counterparty.CounterpartyId),
            new XElement("CriterioPrecificacao", "Título para Negociação"),
            new XElement("DataBoleta", tradeModel.MovementDate),
            new XElement("DataLiquidacao", tradeModel.SettlementDate),
            new XElement("DataRegistro", tradeModel.MovementDate),
            tipoMovimento == "Compra"
                ? new XElement("EntradaFinanceiraBradesco", tradeModel.SettlementBankAccount.ExternalId)
                : null,
            new XElement("InformacoesIncompletas", "Não"),
            new XElement("MercadoSecundario", tradeModel.IsSecondary ? "SIM" : "NÃO"),
            tipoMovimento == "Compra"
                ? new XElement("OrigemAplicacao", tradeModel.TradeReason)
                : null,
            tipoMovimento != "Compra"
                ? new XElement("MotivoResgate", tradeModel.TradeReason)
                : null,
            new XElement("Produto", tradeModel.Product.ExternalProductId),
            new XElement("ProdutoCadastrado", "Sim"),
            tradeModel.Price.HasValue
                ? new XElement("PU", tradeModel.Price)
                : null,
            tradeModel.Quantity.HasValue
                ? new XElement("Quantidade", tradeModel.Quantity)
                : null,
            new XElement("SomenteInformacao", tradeModel.IsForInformationOnly ? "Sim" : "Não"),
            new XElement("TipoBoleta", "Renda Fixa"),
            tipoMovimento != "Compra"
                ? new XElement("TipoContaDestino", "Cadastradas na Conta Movimento Origem")
                : null,
            new XElement("TipoMovimento", tipoMovimento),
            new XElement("Titularidade", tradeModel.PortfolioHolder.ExternalId),
            new XElement("ValorOperacao", tradeModel.FinancialValue)
        );

        return new XElement("BoletaRendaFixa", template);
    }
}