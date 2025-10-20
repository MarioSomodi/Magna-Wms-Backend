using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.LocationAggregate;

namespace MagnaWms.Application.Locations.Repository;

public interface ILocationRepository : IBaseRepository<Location>
{
    Task<IReadOnlyList<Location>> GetByWarehouseIdAsync(long warehouseId, CancellationToken cancellationToken = default);
}
