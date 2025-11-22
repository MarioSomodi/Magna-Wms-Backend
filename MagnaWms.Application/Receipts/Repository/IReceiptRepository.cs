using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.ReceiptAggregate;

namespace MagnaWms.Application.Receipts.Repository;

public interface IReceiptRepository : IBaseRepository<Receipt>
{
    Task<Receipt?> GetWithLinesAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Receipt>> GetByWarehouseIdAsync(long warehouseId, CancellationToken ct);
}
