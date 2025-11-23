using FluentValidation;

namespace MagnaWms.Application.SalesOrders.Commands.CreateSalesOrder;

public sealed class CreateSalesOrderCommandValidator
    : AbstractValidator<CreateSalesOrderCommand>
{
    public CreateSalesOrderCommandValidator()
    {
        RuleFor(x => x.WarehouseId).GreaterThan(0).WithMessage("Warehouse ID must be greater than zero.");
        RuleFor(x => x.OrderNumber).NotEmpty().WithMessage("Order number must be supplied.");
        RuleFor(x => x.CustomerName).NotEmpty().WithMessage("Customer name must be supplied.");

        RuleFor(x => x.Lines)
            .NotEmpty();

        RuleForEach(x => x.Lines)
            .ChildRules(line =>
            {
                line.RuleFor(l => l.ItemId).GreaterThan(0);
                line.RuleFor(l => l.QuantityOrdered).GreaterThan(0);
            });
    }
}
