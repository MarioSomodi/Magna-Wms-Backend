using FluentValidation;

namespace MagnaWms.Application.SalesOrders.Commands.CancelSalesOrder;

public sealed class CancelSalesOrderCommandValidator
    : AbstractValidator<CancelSalesOrderCommand>
{
    public CancelSalesOrderCommandValidator() => RuleFor(x => x.SalesOrderId).GreaterThan(0).WithMessage("Sales order ID must be greater than zero.");
}
