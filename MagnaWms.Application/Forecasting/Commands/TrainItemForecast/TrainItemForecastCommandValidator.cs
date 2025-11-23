using FluentValidation;

namespace MagnaWms.Application.Forecasting.Commands.TrainItemForecast;

public sealed class TrainItemForecastCommandValidator : AbstractValidator<TrainItemForecastCommand>
{
    public TrainItemForecastCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .GreaterThan(0)
            .WithMessage("Warehouse ID must be greater than zero.");

        RuleFor(x => x.ItemId)
            .GreaterThan(0)
            .WithMessage("Item ID must be greater than zero.");

        RuleFor(x => x.HorizonDays)
            .InclusiveBetween(1, 365)
            .WithMessage("Horizon must be between 1 and 365 days.");
    }
}
