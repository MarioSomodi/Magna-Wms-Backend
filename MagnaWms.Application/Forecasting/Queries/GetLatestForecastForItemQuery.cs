using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Forecasting.Queries.GetLatestForecastForItem;

public sealed record GetLatestForecastForItemQuery(
    long WarehouseId,
    long ItemId,
    int HorizonDays
) : IRequest<Result<ForecastSeriesDto>>;
