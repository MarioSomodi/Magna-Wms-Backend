using FluentValidation;

namespace MagnaWms.Application.Receipts.Commands.ReceiveReceiptLine;

public sealed class ReceiveReceiptLineCommandValidator : AbstractValidator<ReceiveReceiptLineCommand>
{
    public ReceiveReceiptLineCommandValidator()
    {
        RuleFor(x => x.ReceiptId)
            .GreaterThan(0).WithMessage("Receipt ID must be greater than zero.");

        RuleFor(x => x.LineId)
            .GreaterThan(0).WithMessage("Line ID must be greater than zero.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.ToLocationId)
            .GreaterThan(0).WithMessage("Location ID must be greater than zero.");
    }
}
