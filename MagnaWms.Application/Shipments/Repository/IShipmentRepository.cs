using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.ShipmentAggregate;

namespace MagnaWms.Application.Shipments.Repository;

public interface IShipmentRepository : IBaseRepository<Shipment>
{
    Task<Shipment?> GetWithLinesAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Shipment>> GetByWarehouseAsync(
        long warehouseId,
        CancellationToken cancellationToken = default);
}
