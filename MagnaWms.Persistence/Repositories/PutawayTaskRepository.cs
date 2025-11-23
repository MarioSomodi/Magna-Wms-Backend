using MagnaWms.Application.Putaway.Repository;
using MagnaWms.Domain;
using MagnaWms.Domain.PutawayAggregate;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;

public sealed class PutawayTaskRepository
    : BaseRepository<PutawayTask>, IPutawayTaskRepository
{
    public PutawayTaskRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<PutawayTask>> GetOpenTasksForWarehouseAsync(
        long warehouseId,
        CancellationToken cancellationToken = default) => await Context.Set<PutawayTask>()
            .AsNoTracking()
            .Where(t => t.WarehouseId == warehouseId &&
                        t.Status == OCStatus.Open)
            .ToListAsync(cancellationToken);
}
