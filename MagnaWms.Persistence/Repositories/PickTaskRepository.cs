using MagnaWms.Application.PickTasks.Repository;
using MagnaWms.Domain.PickTaskAggregate;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;

public sealed class PickTaskRepository : BaseRepository<PickTask>, IPickTaskRepository
{
    public PickTaskRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PickTask?> GetWithLinesAsync(long id, CancellationToken cancellationToken = default) =>
        await Context.Set<PickTask>()
            .Include(p => p.Lines)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<PickTask>> GetOpenByWarehouseAsync(
        long warehouseId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<PickTask>()
            .AsNoTracking()
            .Where(p => p.WarehouseId == warehouseId && p.Status == PickTaskStatus.Open)
            .OrderBy(p => p.CreatedUtc)
            .ToListAsync(cancellationToken);
}
