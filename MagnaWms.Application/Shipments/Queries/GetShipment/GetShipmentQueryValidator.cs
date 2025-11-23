using FluentValidation;

namespace MagnaWms.Application.Shipments.Queries.GetShipment;

public sealed class GetShipmentQueryValidator : AbstractValidator<GetShipmentQuery>
{
    public GetShipmentQueryValidator() => RuleFor(x => x.ShipmentId).GreaterThan(0);
}
