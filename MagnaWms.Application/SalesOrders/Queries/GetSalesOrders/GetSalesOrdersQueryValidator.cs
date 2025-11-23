using FluentValidation;

namespace MagnaWms.Application.SalesOrders.Queries.GetSalesOrders;

public sealed class GetSalesOrdersQueryValidator
    : AbstractValidator<GetSalesOrdersQuery>
{
    public GetSalesOrdersQueryValidator() => RuleFor(x => x.WarehouseId).GreaterThan(0).WithMessage("Warehouse ID must be greater than zero.");
}
