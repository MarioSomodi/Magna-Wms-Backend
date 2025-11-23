using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.LocationAggregate;

namespace MagnaWms.Application.Locations.Repository;

public interface ILocationRepository : IBaseRepository<Location>
{
    Task<IReadOnlyList<Location>> GetByType(string type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Location>> GetStageLocationForWarehouse(long warehouseId, CancellationToken cancellationToken = default)
    Task<bool> ExistsInWarehouse(long locationId, long warehouseId, CancellationToken ct);
    Task<IReadOnlyList<Location>> GetByWarehouseIdAsync(long warehouseId, CancellationToken cancellationToken = default);
}
