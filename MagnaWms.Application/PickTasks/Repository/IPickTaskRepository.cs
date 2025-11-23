using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.PickTaskAggregate;

namespace MagnaWms.Application.PickTasks.Repository;

public interface IPickTaskRepository : IBaseRepository<PickTask>
{
    Task<PickTask?> GetWithLinesAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PickTask>> GetOpenByWarehouseAsync(
        long warehouseId,
        CancellationToken cancellationToken = default);
}
