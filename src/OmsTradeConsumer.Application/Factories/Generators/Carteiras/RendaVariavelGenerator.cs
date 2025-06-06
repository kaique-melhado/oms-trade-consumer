﻿using OmsTradeConsumer.Domain.Models;
using System.Xml.Linq;

namespace OmsTradeConsumer.Application.Factories.Generators.Carteiras;

public class RendaVariavelGenerator : IXmlGenerator
{
    public XElement Generate(TradeModel tradeModel)
    {
        if (tradeModel == null) throw new ArgumentNullException(nameof(tradeModel));
        if (tradeModel.CustodyBankAccount == null) throw new ArgumentNullException(nameof(tradeModel.CustodyBankAccount));
        if (tradeModel.Product == null) throw new ArgumentNullException(nameof(tradeModel.Product));
        if (tradeModel.Order == null) throw new ArgumentNullException(nameof(tradeModel.Order));
        if (tradeModel.PortfolioHolder == null) throw new ArgumentNullException(nameof(tradeModel.PortfolioHolder));

        var tipoMovimento = tradeModel.Order.MovementTypeDescription;

        var template = new XElement("TemplateBoletaRendaVariavelUpload",
            new XElement("Acao", "INCLUIR"),
            new XElement("CodigoExternoAtivo", tradeModel.Product.ExternalProductCode),
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
            tipoMovimento == "Compra"
                ? new XElement("OrigemAplicacao", tradeModel.TradeReason)
                : null,
            tipoMovimento != "Compra"
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
            new XElement("TipoBoleta", "Renda Variável - Ação e Opção"),
            tipoMovimento != "Compra"
                ? new XElement("TipoContaDestino", "Cadastradas na Conta Movimento Origem")
                : null,
            new XElement("TipoMovimento", tipoMovimento),
            new XElement("Titularidade", tradeModel.PortfolioHolder.ExternalId),
            new XElement("ValorOperacao", tradeModel.FinancialValue)
        );

        return new XElement("BoletaRendaVariavel", template);
    }
}