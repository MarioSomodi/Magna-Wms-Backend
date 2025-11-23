using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.ForecastAggregate;

namespace MagnaWms.Application.Forecasting.Repository;

public interface IForecastSeriesRepository : IBaseRepository<ForecastSeries>
{
    Task<ForecastSeries?> GetLatestForItemAsync(
        long warehouseId,
        long itemId,
        int horizonDays,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ForecastSeries>> GetAllForItemAsync(
        long warehouseId,
        long itemId,
        int horizonDays,
        CancellationToken cancellationToken = default);
}
