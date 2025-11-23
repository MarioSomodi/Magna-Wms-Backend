using FluentValidation;

namespace MagnaWms.Application.Forecasting.Commands.TrainAllItemsForecast;

public sealed class TrainAllItemsForecastCommandValidator
    : AbstractValidator<TrainAllItemsForecastCommand>
{
    public TrainAllItemsForecastCommandValidator()
    {
        RuleFor(x => x.WarehouseId).GreaterThan(0);
        RuleFor(x => x.HorizonDays).InclusiveBetween(7, 365);
    }
}
