using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.LocationAggregate;

namespace MagnaWms.Application.Locations.Repository;

public interface ILocationRepository : IBaseRepository<Location>
{
    Task<bool> ExistsInWarehouse(long locationId, long warehouseId, CancellationToken ct);
    Task<IReadOnlyList<Location>> GetByWarehouseIdAsync(long warehouseId, CancellationToken cancellationToken = default);
}
