using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.Authorization;
public sealed class UserWarehouse : AggregateRoot
{
    public long UserId { get; private set; }
    public long WarehouseId { get; private set; }

    private UserWarehouse() { }

    public UserWarehouse(long userId, long warehouseId)
    {
        UserId = userId;
        WarehouseId = warehouseId;
    }
}
