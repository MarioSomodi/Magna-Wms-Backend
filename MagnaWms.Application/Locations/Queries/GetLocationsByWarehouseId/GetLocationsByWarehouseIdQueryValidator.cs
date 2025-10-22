using FluentValidation;
using MagnaWms.Application.Warehouses.Queries.GetWarehouseById;

namespace MagnaWms.Application.Locations.Queries.GetLocationsByWarehouseId;
public class GetLocationsByWarehouseIdQueryValidator : AbstractValidator<GetLocationsByWarehouseIdQuery>
{
    public GetLocationsByWarehouseIdQueryValidator() => RuleFor(x => x.WarehouseId)
            .GreaterThan(0)
            .WithMessage("Warehouse ID must be greater than zero.");
}
