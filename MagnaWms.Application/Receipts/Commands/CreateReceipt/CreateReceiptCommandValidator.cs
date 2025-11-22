using FluentValidation;

namespace MagnaWms.Application.Receipts.Commands.CreateReceipt;

public sealed class CreateReceiptCommandValidator : AbstractValidator<CreateReceiptCommand>
{
    public CreateReceiptCommandValidator()
    {
        RuleFor(x => x.WarehouseId).GreaterThan(0);
        RuleFor(x => x.ReceiptNumber).NotEmpty().MaximumLength(64);

        RuleForEach(x => x.Lines).ChildRules(lines =>
        {
            lines.RuleFor(l => l.ItemId).GreaterThan(0);
            lines.RuleFor(l => l.ExpectedQty).GreaterThan(0);
        });
    }
}
