using FluentValidation;

namespace MagnaWms.Application.Receipts.Queries.GetReceipt;

public sealed class GetReceiptQueryValidator : AbstractValidator<GetReceiptQuery>
{
    public GetReceiptQueryValidator() =>
        RuleFor(x => x.ReceiptId).GreaterThan(0).WithMessage("Receipt ID must be grater than 0.");
}
