using MagnaWms.Application.Items.Repository;
using MagnaWms.Domain.ItemAggregate;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;

public class ItemRepository : BaseRepository<Item>, IItemRepository
{
    public ItemRepository(AppDbContext context) : base(context) { }

    public async Task<Item?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
        => await Context.Set<Item>()
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Sku == sku, cancellationToken);
}
