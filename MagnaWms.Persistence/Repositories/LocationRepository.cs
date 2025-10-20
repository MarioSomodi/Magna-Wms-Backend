using MagnaWms.Application.Locations.Repository;
using MagnaWms.Domain.LocationAggregate;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;

public class LocationRepository : BaseRepository<Location>, ILocationRepository
{
    public LocationRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Location>> GetByWarehouseIdAsync(long warehouseId, CancellationToken cancellationToken = default)
        => await Context.Set<Location>()
            .AsNoTracking()
            .Where(l => l.WarehouseId == warehouseId)
            .ToListAsync(cancellationToken);
}
