using FluentValidation;

namespace MagnaWms.Application.Shipments.Commands.CreateShipment;

public sealed class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
{
    public CreateShipmentCommandValidator()
    {
        RuleFor(x => x.SalesOrderId).GreaterThan(0);
        RuleFor(x => x.ShipmentNumber).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Carrier).NotEmpty().MaximumLength(128);
        RuleFor(x => x.TrackingNumber).NotEmpty().MaximumLength(128);

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("Shipment must contain at least one line.");

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.ItemId).GreaterThan(0);
            line.RuleFor(l => l.Quantity).GreaterThan(0);
        });
    }
}
