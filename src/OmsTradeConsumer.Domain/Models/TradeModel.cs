namespace OmsTradeConsumer.Domain.Models;

public class TradeModel
{
    public int TradeBlotterId { get; set; }
    public string ExternalTradeId { get; set; }
    public DateTime MovementDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime QuotationDate { get; set; }
    public DateTime SettlementDate { get; set; }
    public decimal FinancialValue { get; set; }
    public int? Quantity { get; set; }
    public decimal? Price { get; set; }
    public bool IsSecondary { get; set; }
    public bool IsExported { get; set; }
    public bool IsDeleted { get; set; }
    public bool HasLiquidity { get; set; }
    public bool IsSeparable { get; set; }
    public bool IsRollover { get; set; }
    public string ExternalRolloverTypeId { get; set; }
    public decimal? ShortTipRolloverPrice { get; set; }
    public decimal? LongTipRolloverPrice { get; set; }
    public int ExecutedQuantity { get; set; }
    public int StatusCode { get; set; }
    public string StatusDescription { get; set; }
    public string User { get; set; }
    public OrderModel Order { get; set; }
    public CounterpartyModel Counterparty { get; set; }
    public CounterpartyModel ExternalCounterparty { get; set; }
    public ProductModel Product { get; set; }
    public ProductModel ShortTipRolloverProduct { get; set; }
    public ProductModel LongTipRolloverProduct { get; set; }
    public PortfolioModel PortfolioHolder { get; set; }
    public BankAccountModel CustodyBankAccount { get; set; }
    public BankAccountModel SettlementBankAccount { get; set; }
    public string TradeReason { get; set; }
    public bool IsForInformationOnly { get; set; }
}

public class OrderModel
{
    public int OrderId { get; set; }
    public int OrderType { get; set; }
    public string OrderTypeDescription { get; set; }
    public int MovementType { get; set; }
    public string MovementTypeDescription { get; set; }
    public int ApplicationType { get; set; }
    public string ApplicationTypeDescription { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public decimal FinancialValue { get; set; }
    public int Quantity { get; set; }
    public string ActionDescription { get; set; }
    public string Note { get; set; }
    public int OrderStatus { get; set; }
    public string OrderStatusDescription { get; set; }
    public bool IsOrderFromClient { get; set; }
    public string OriginSystem { get; set; }
    public string TargetSystem { get; set; }
    public string User { get; set; }
    public ClientModel Client { get; set; }
}

public class ClientModel
{
    public string ExternalClientId { get; set; }
    public int ClientIdentificationType { get; set; }
    public string ClientIdentificationTypeDescription { get; set; }
    public string ClientName { get; set; }
}

public class CounterpartyModel
{
    public string CounterpartyId { get; set; }
    public string CounterpartyName { get; set; }
}

public class ProductModel
{
    public string ExternalProductId { get; set; }
    public string ExternalProductCode { get; set; }
    public string ProductName { get; set; }
    public int ProductType { get; set; }
    public string ProductTypeDescription { get; set; }
    public string Issuer { get; set; }
    public string ExternalIssuerId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string Indexer { get; set; }
    public decimal IndexerPercentage { get; set; }
    public decimal Coupom { get; set; }
}

public class PortfolioModel
{
    public string ExternalId { get; set; }
    public string Name { get; set; }
}

public class BankAccountModel
{
    public string ExternalId { get; set; }
    public string Name { get; set; }
}