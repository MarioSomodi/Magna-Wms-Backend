using MagnaWms.Application.SalesOrder.Repository;
using MagnaWms.Domain.SalesOrderAggregate;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;

public sealed class SalesOrderRepository : BaseRepository<SalesOrder>, ISalesOrderRepository
{
    public SalesOrderRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<SalesOrder?> GetWithLinesAsync(long id, CancellationToken cancellationToken = default) =>
        await Context.Set<SalesOrder>()
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<IReadOnlyList<SalesOrder>> GetByWarehouseAsync(
        long warehouseId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<SalesOrder>()
            .AsNoTracking()
            .Where(o => o.WarehouseId == warehouseId)
            .OrderByDescending(o => o.CreatedUtc)
            .ToListAsync(cancellationToken);
}
