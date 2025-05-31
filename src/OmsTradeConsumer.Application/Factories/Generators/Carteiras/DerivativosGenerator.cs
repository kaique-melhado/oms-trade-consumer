using OmsTradeConsumer.Domain.Models;
using System.Xml.Linq;

namespace OmsTradeConsumer.Application.Factories.Generators.Carteiras;

public class DerivativosGenerator : IXmlGenerator
{
    public XElement Generate(TradeModel tradeModel)
    {
        if (tradeModel == null) throw new ArgumentNullException(nameof(tradeModel));
        if (tradeModel.CustodyBankAccount == null) throw new ArgumentNullException(nameof(tradeModel.CustodyBankAccount));
        if (tradeModel.Product == null) throw new ArgumentNullException(nameof(tradeModel.Product));
        if (tradeModel.Order == null) throw new ArgumentNullException(nameof(tradeModel.Order));
        if (tradeModel.PortfolioHolder == null) throw new ArgumentNullException(nameof(tradeModel.PortfolioHolder));

        var tipoMovimento = tradeModel.Order.MovementTypeDescription;
        var isRollover = tradeModel.IsRollover;

        var template = new XElement("TemplateBoletaDerivativosUpload",
            new XElement("Acao", "INCLUIR"),
            new XElement("ContaMovimentoOrigem", tradeModel.CustodyBankAccount.ExternalId),
            tipoMovimento != "Compra"
                ? new XElement("ContaMovimentoDestino", tradeModel.SettlementBankAccount.ExternalId)
                : null,
            new XElement("Corretora", tradeModel.Counterparty.CounterpartyId),
            new XElement("CorretoraExterna", tradeModel.ExternalCounterparty.CounterpartyId),
            new XElement("DataBoleta", tradeModel.MovementDate),
            new XElement("DataLiquidacao", tradeModel.SettlementDate),
            new XElement("DataRegistro", tradeModel.MovementDate),
            tipoMovimento == "Compra"
                ? new XElement("EntradaFinanceiraBradesco", tradeModel.SettlementBankAccount.ExternalId)
                : null,
            new XElement("ERolagem", isRollover ? "Sim" : "Não"),
            isRollover
                ? new XElement("TipoRolagem", tradeModel.ExternalRolloverTypeId)
                : null,
            tipoMovimento == "Compra"
                ? new XElement("OrigemAplicacao", tradeModel.TradeReason)
                : null,
            tipoMovimento != "Compra"
                ? new XElement("MotivoResgate", tradeModel.TradeReason)
                : null,
            new XElement("Produto", tradeModel.Product.ExternalProductId),
            isRollover
                ? new XElement("ProdutoPontaCurta", tradeModel.ShortTipRolloverProduct?.ExternalProductId)
                : null,
            isRollover
                ? new XElement("ProdutoPontaLonga", tradeModel.LongTipRolloverProduct?.ExternalProductId)
                : null,
            tradeModel.Price.HasValue
                ? new XElement("PU", tradeModel.Price)
                : null,
            isRollover && tradeModel.ShortTipRolloverPrice.HasValue
                ? new XElement("PuPontaCurta", tradeModel.ShortTipRolloverPrice)
                : null,
            isRollover && tradeModel.LongTipRolloverPrice.HasValue
                ? new XElement("PuPontaLonga", tradeModel.LongTipRolloverPrice)
                : null,
            tradeModel.Quantity.HasValue
                ? new XElement("Quantidade", tradeModel.Quantity)
                : null,
            new XElement("SomenteInformacao", tradeModel.IsForInformationOnly ? "Sim" : "Não"),
            new XElement("TipoBoleta", "Derivativos - Futuros e Opções"),
            tipoMovimento != "Compra"
                ? new XElement("TipoContaDestino", "Cadastradas na Conta Movimento Origem")
                : null,
            new XElement("TipoMovimento", tipoMovimento),
            new XElement("Titularidade", tradeModel.PortfolioHolder.ExternalId),
            new XElement("ValorOperacao", tradeModel.FinancialValue)
        );

        return new XElement("BoletaDerivativos", template);
    }
}