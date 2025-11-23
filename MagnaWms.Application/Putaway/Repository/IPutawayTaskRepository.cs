using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.PutawayAggregate;

namespace MagnaWms.Application.Putaway.Repository;

public interface IPutawayTaskRepository : IBaseRepository<PutawayTask>
{
    Task<IReadOnlyList<PutawayTask>> GetOpenTasksForWarehouseAsync(
        long warehouseId,
        CancellationToken cancellationToken = default
    );
}
