using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.InventoryAggregate;

namespace MagnaWms.Application.InventoryLedgers.Repository;

public interface IInventoryLedgerRepository : IBaseRepository<InventoryLedgerEntry>
{
    Task<IReadOnlyList<InventoryLedgerEntry>> GetForItemAsync(
        long warehouseId,
        long itemId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryLedgerEntry>> GetForItemAtLocationAsync(
        long warehouseId,
        long locationId,
        long itemId,
        CancellationToken cancellationToken = default);
}
