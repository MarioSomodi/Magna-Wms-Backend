using FluentValidation;

namespace MagnaWms.Application.Forecasting.Queries.GetLatestForecastForItem;

public sealed class GetLatestForecastForItemQueryValidator : AbstractValidator<GetLatestForecastForItemQuery>
{
    public GetLatestForecastForItemQueryValidator()
    {
        RuleFor(x => x.WarehouseId).GreaterThan(0);
        RuleFor(x => x.ItemId).GreaterThan(0);
        RuleFor(x => x.HorizonDays).GreaterThan(0);
    }
}
