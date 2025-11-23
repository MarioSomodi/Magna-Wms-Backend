using FluentValidation;

namespace MagnaWms.Application.SalesOrders.Commands.AllocateSalesOrder;

public sealed class AllocateSalesOrderCommandValidator
    : AbstractValidator<AllocateSalesOrderCommand>
{
    public AllocateSalesOrderCommandValidator() => RuleFor(x => x.SalesOrderId).GreaterThan(0).WithMessage("Sales order ID must be greater than zero.");
}
