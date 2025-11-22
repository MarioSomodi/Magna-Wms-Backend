using FluentValidation;

namespace MagnaWms.Application.Receipts.Queries.GetReceiptLines;

public sealed class GetReceiptLinesQueryValidator : AbstractValidator<GetReceiptLinesQuery>
{
    public GetReceiptLinesQueryValidator() =>
        RuleFor(x => x.ReceiptId).GreaterThan(0).WithMessage("Receipt ID must be grater than 0.");
}
