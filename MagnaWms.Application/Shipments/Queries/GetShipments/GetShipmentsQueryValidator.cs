using FluentValidation;

namespace MagnaWms.Application.Shipments.Queries.GetShipments;

public sealed class GetShipmentsQueryValidator : AbstractValidator<GetShipmentsQuery>
{
    public GetShipmentsQueryValidator() => RuleFor(x => x.WarehouseId).GreaterThan(0);
}
