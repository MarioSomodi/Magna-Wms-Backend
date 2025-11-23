using FluentValidation;

namespace MagnaWms.Application.Replenishment.Queries.GetWarehouseSuggestions;

public sealed class GetReplenishmentSuggestionsQueryValidator
    : AbstractValidator<GetReplenishmentSuggestionsQuery>
{
    public GetReplenishmentSuggestionsQueryValidator()
    {
        RuleFor(x => x.WarehouseId).GreaterThan(0);
        RuleFor(x => x.HorizonDays).InclusiveBetween(7, 365);
    }
}
