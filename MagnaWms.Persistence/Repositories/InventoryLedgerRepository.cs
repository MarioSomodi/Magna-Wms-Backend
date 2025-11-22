using MagnaWms.Application.InventoryLedgers.Repository;
using MagnaWms.Domain.InventoryAggregate;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;

public sealed class InventoryLedgerRepository : BaseRepository<InventoryLedgerEntry>, IInventoryLedgerRepository
{
    public InventoryLedgerRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<InventoryLedgerEntry>> GetForItemAsync(
        long warehouseId,
        long itemId,
        CancellationToken cancellationToken = default)
        => await Context.Set<InventoryLedgerEntry>()
            .AsNoTracking()
            .Where(e => e.WarehouseId == warehouseId && e.ItemId == itemId)
            .OrderBy(e => e.TimestampUtc)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<InventoryLedgerEntry>> GetForItemAtLocationAsync(
        long warehouseId,
        long locationId,
        long itemId,
        CancellationToken cancellationToken = default)
        => await Context.Set<InventoryLedgerEntry>()
            .AsNoTracking()
            .Where(e =>
                e.WarehouseId == warehouseId &&
                e.LocationId == locationId &&
                e.ItemId == itemId)
            .OrderBy(e => e.TimestampUtc)
            .ToListAsync(cancellationToken);
}
