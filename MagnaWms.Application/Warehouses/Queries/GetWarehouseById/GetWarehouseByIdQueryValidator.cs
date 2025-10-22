using FluentValidation;

namespace MagnaWms.Application.Warehouses.Queries.GetWarehouseById;

public sealed class GetWarehouseByIdQueryValidator : AbstractValidator<GetWarehouseByIdQuery>
{
    public GetWarehouseByIdQueryValidator() => RuleFor(x => x.WarehouseId)
            .GreaterThan(0)
            .WithMessage("Warehouse ID must be greater than zero.");
}
