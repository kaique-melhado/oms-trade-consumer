using FluentValidation;
using OmsTradeConsumer.Domain.Models;

namespace OmsTradeConsumer.Application.Validators;

public class TradeModelValidator : AbstractValidator<TradeModel>
{
    public TradeModelValidator()
    {
        RuleFor(trade => trade.TradeBlotterId)
            .GreaterThan(0).WithMessage("O TradeBlotterId deve ser maior que 0.");

        RuleFor(trade => trade.ExternalTradeId)
            .NotEmpty().WithMessage("O ExternalTradeId é obrigatório.")
            .MaximumLength(50).WithMessage("O ExternalTradeId não pode ter mais de 50 caracteres.");

        RuleFor(trade => trade.MovementDate)
            .NotEmpty().WithMessage("A MovementDate é obrigatória.");

        RuleFor(trade => trade.ExpirationDate)
            .GreaterThanOrEqualTo(trade => trade.MovementDate)
            .WithMessage("A ExpirationDate deve ser posterior ou igual à MovementDate.");

        RuleFor(trade => trade.FinancialValue)
            .GreaterThanOrEqualTo(0).WithMessage("O FinancialValue deve ser maior ou igual a 0.");

        RuleFor(trade => trade.Quantity)
            .GreaterThan(0).WithMessage("O Quantity deve ser maior que 0.");

        RuleFor(trade => trade.Price)
            .GreaterThanOrEqualTo(0).WithMessage("O Price deve ser maior ou igual a 0.");

        RuleFor(trade => trade.Order)
            .NotNull().WithMessage("A Order é obrigatória.")
            .SetValidator(new OrderValidator());

        RuleFor(trade => trade.Product)
            .NotNull().WithMessage("O Product é obrigatório.")
            .SetValidator(new ProductValidator());

        RuleFor(trade => trade.PortfolioHolder)
            .NotNull().WithMessage("O PortfolioHolder é obrigatório.")
            .SetValidator(new PortfolioHolderValidator());

        RuleFor(trade => trade.CustodyBankAccount)
            .NotNull().WithMessage("A CustodyBankAccount é obrigatória.")
            .SetValidator(new BankAccountValidator());

        RuleFor(trade => trade.SettlementBankAccount)
            .NotNull().WithMessage("A SettlementBankAccount é obrigatória.")
            .SetValidator(new BankAccountValidator());
    }
}

public class OrderValidator : AbstractValidator<OrderModel>
{
    public OrderValidator()
    {
        RuleFor(order => order.OrderId)
            .GreaterThan(0).WithMessage("O OrderId deve ser maior que 0.");

        RuleFor(order => order.OrderTypeDescription)
            .NotEmpty().WithMessage("O OrderTypeDescription é obrigatório.");

        RuleFor(order => order.Client)
            .NotNull().WithMessage("O Client é obrigatório.")
            .SetValidator(new ClientValidator());
    }
}

public class ProductValidator : AbstractValidator<ProductModel>
{
    public ProductValidator()
    {
        RuleFor(product => product.ExternalProductId)
            .NotEmpty().WithMessage("O ExternalProductId é obrigatório.");

        RuleFor(product => product.ProductName)
            .NotEmpty().WithMessage("O ProductName é obrigatório.");
    }
}

public class ClientValidator : AbstractValidator<ClientModel>
{
    public ClientValidator()
    {
        RuleFor(client => client.ExternalClientId)
            .NotEmpty().WithMessage("O ExternalClientId é obrigatório.");

        RuleFor(client => client.ClientName)
            .NotEmpty().WithMessage("O ClientName é obrigatório.");
    }
}

public class PortfolioHolderValidator : AbstractValidator<PortfolioModel>
{
    public PortfolioHolderValidator()
    {
        RuleFor(holder => holder.ExternalId)
            .NotEmpty().WithMessage("O ExternalId é obrigatório.");

        RuleFor(holder => holder.Name)
            .NotEmpty().WithMessage("O Name é obrigatório.");
    }
}

public class BankAccountValidator : AbstractValidator<BankAccountModel>
{
    public BankAccountValidator()
    {
        RuleFor(account => account.ExternalId)
            .NotEmpty().WithMessage("O ExternalId é obrigatório.");

        RuleFor(account => account.Name)
            .NotEmpty().WithMessage("O Name é obrigatório.");
    }
}
