using MagnaWms.Application.Receipts.Repository;
using MagnaWms.Domain.ReceiptAggregate;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;

public sealed class ReceiptRepository : BaseRepository<Receipt>, IReceiptRepository
{
    public ReceiptRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Receipt?> GetWithLinesAsync(long id, CancellationToken cancellationToken = default)
        => await Context.Set<Receipt>()
            .Include(r => r.Lines)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    
    public async Task<IReadOnlyList<Receipt>> GetByWarehouseIdAsync(long warehouseId, CancellationToken ct) =>
        await Context.Set<Receipt>()
            .Include(r => r.Lines)
            .Where(r => r.WarehouseId == warehouseId)
            .OrderByDescending(r => r.CreatedUtc)
            .ToListAsync(ct);
}
