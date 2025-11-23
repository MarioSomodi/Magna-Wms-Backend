using MagnaWms.Application.Shipments.Repository;
using MagnaWms.Domain.ShipmentAggregate;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;

public sealed class ShipmentRepository : BaseRepository<Shipment>, IShipmentRepository
{
    public ShipmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Shipment?> GetWithLinesAsync(long id, CancellationToken cancellationToken = default) =>
        await Context.Set<Shipment>()
            .Include(s => s.Lines)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Shipment>> GetByWarehouseAsync(
        long warehouseId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<Shipment>()
            .AsNoTracking()
            .Where(s => s.WarehouseId == warehouseId)
            .OrderByDescending(s => s.ShippedUtc)
            .ToListAsync(cancellationToken);
}
