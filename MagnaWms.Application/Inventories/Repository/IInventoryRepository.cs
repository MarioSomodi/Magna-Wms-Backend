using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.InventoryAggregate;

namespace MagnaWms.Application.Inventories.Repository;

public interface IInventoryRepository : IBaseRepository<Inventory>
{
    Task<Inventory?> GetByKeyAsync(
        long warehouseId,
        long locationId,
        long itemId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Inventory>> GetByWarehouseAsync(
        long warehouseId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Inventory>> GetByWarehousesAsync(
        IReadOnlyList<long> warehouseIds,
        CancellationToken cancellationToken = default);
}
