using FluentValidation;

namespace MagnaWms.Application.Replenishment.Queries.GetReplenishmentSuggestionForItem;

public sealed class GetReplenishmentSuggestionForItemQueryValidator
    : AbstractValidator<GetReplenishmentSuggestionForItemQuery>
{
    public GetReplenishmentSuggestionForItemQueryValidator()
    {
        RuleFor(x => x.WarehouseId).GreaterThan(0);
        RuleFor(x => x.ItemId).GreaterThan(0);
        RuleFor(x => x.HorizonDays).InclusiveBetween(7, 365);
    }
}
