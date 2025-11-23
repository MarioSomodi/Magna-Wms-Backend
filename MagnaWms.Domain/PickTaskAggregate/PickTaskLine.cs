using MagnaWms.Domain.Core.Exceptions;
using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.PickTaskAggregate;

public sealed class PickTaskLine : Entity
{
    private PickTaskLine() { }

    public PickTaskLine(long pickTaskId, long itemId, long locationId, decimal quantityToPick)
    {
        if (quantityToPick <= 0)
        {
            throw new DomainException("Quantity to pick must be positive.");
        }

        PickTaskId = pickTaskId;
        ItemId = itemId;
        LocationId = locationId;
        QuantityToPick = quantityToPick;
        QuantityPicked = 0m;
    }

    public long PickTaskId { get; private set; }
    public long ItemId { get; private set; }
    public long LocationId { get; private set; }

    public decimal QuantityToPick { get; private set; }
    public decimal QuantityPicked { get; private set; }

    public bool IsCompleted => QuantityPicked >= QuantityToPick;

    public void Pick(decimal qty)
    {
        if (qty <= 0)
        {
            throw new DomainException("Pick quantity must be positive.");
        }

        decimal newPicked = QuantityPicked + qty;

        if (newPicked > QuantityToPick)
        {
            throw new DomainException(
                $"Cannot pick {qty}. Would exceed task quantity {QuantityToPick}.");
        }

        QuantityPicked = newPicked;
    }
}
