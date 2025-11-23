using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.SalesOrderAggregate;

namespace MagnaWms.Application.SalesOrders.Repository;

public interface ISalesOrderRepository : IBaseRepository<SalesOrder>
{
    Task<SalesOrder?> GetWithLinesAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SalesOrder>> GetByWarehouseAsync(
        long warehouseId,
        CancellationToken cancellationToken = default);
}
