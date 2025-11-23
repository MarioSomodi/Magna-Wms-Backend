using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Forecasting.Commands.TrainItemForecast;

public sealed record TrainItemForecastCommand(
    long WarehouseId,
    long ItemId,
    int HorizonDays
) : IRequest<Result<ForecastSeriesDto>>;
