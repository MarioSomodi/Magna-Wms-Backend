using MagnaWms.Application.Inventories.Repository;
using MagnaWms.Domain.InventoryAggregate;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;

public sealed class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
{
    public InventoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Inventory?> GetByKeyAsync(
        long warehouseId,
        long locationId,
        long itemId,
        CancellationToken cancellationToken = default)
        => await Context.Set<Inventory>()
            .FirstOrDefaultAsync(
                i => i.WarehouseId == warehouseId
                  && i.LocationId == locationId
                  && i.ItemId == itemId,
                cancellationToken);

    public async Task<IReadOnlyList<Inventory>> GetByWarehouseAsync(
        long warehouseId,
        CancellationToken cancellationToken = default)
        => await Context.Set<Inventory>()
            .AsNoTracking()
            .Where(i => i.WarehouseId == warehouseId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Inventory>> GetByWarehousesAsync(
        IReadOnlyList<long> warehouseIds,
        CancellationToken cancellationToken = default)
        => await Context.Set<Inventory>()
            .AsNoTracking()
            .Where(i => warehouseIds.Contains(i.WarehouseId))
            .ToListAsync(cancellationToken);
}
