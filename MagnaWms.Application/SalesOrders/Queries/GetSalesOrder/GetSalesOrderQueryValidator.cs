using FluentValidation;

namespace MagnaWms.Application.SalesOrders.Queries.GetSalesOrder;

public sealed class GetSalesOrderQueryValidator
    : AbstractValidator<GetSalesOrderQuery>
{
    public GetSalesOrderQueryValidator() => RuleFor(x => x.SalesOrderId).GreaterThan(0).WithMessage("Sales order ID must be greater than zero.");
}
