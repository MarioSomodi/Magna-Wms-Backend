using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Forecasting.Commands.TrainAllItemsForecast;

public sealed record TrainAllItemsForecastCommand(
    long WarehouseId,
    int HorizonDays
) : IRequest<Result<TrainForecastResultDto>>;
