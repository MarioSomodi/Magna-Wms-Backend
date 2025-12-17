using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.WarehouseAggregate;

namespace MagnaWms.Application.Warehouses.Repository;
public interface IWarehouseRepository : IBaseRepository<Warehouse>
{
    Task<Warehouse?> ExistsByCodeAsync(string code, CancellationToken cancellationToken);
}
