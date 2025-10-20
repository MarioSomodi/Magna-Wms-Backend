using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.ItemAggregate;

namespace MagnaWms.Application.Items.Repository;

public interface IItemRepository : IBaseRepository<Item>
{
    Task<Item?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
}
