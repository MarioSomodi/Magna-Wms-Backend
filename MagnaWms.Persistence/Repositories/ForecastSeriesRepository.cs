using MagnaWms.Application.Forecasting.Repository;
using MagnaWms.Domain.ForecastAggregate;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;

public sealed class ForecastSeriesRepository : BaseRepository<ForecastSeries>, IForecastSeriesRepository
{
    public ForecastSeriesRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ForecastSeries?> GetLatestForItemAsync(
        long warehouseId,
        long itemId,
        int horizonDays,
        CancellationToken cancellationToken = default) =>
        await Context.Set<ForecastSeries>()
            .Include(f => f.Points)
            .Where(f => f.WarehouseId == warehouseId && f.ItemId == itemId && f.HorizonDays == horizonDays)
            .OrderByDescending(f => f.CreatedUtc)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<ForecastSeries>> GetAllForItemAsync(
        long warehouseId,
        long itemId,
        int horizonDays,
        CancellationToken cancellationToken = default) =>
        await Context.Set<ForecastSeries>()
            .Include(f => f.Points)
            .Where(f => f.WarehouseId == warehouseId && f.ItemId == itemId && f.HorizonDays == horizonDays)
            .OrderByDescending(f => f.CreatedUtc)
            .ToListAsync(cancellationToken);
}
