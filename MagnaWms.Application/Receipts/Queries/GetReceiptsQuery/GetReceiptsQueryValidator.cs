using FluentValidation;

namespace MagnaWms.Application.Receipts.Queries.GetReceiptsQuery;

public sealed class GetReceiptsQueryValidator : AbstractValidator<GetReceiptsQuery>
{
    public GetReceiptsQueryValidator() =>
        RuleFor(x => x.WarehouseId).GreaterThan(0).WithMessage("Warehouse ID must be grater than 0.");
}
