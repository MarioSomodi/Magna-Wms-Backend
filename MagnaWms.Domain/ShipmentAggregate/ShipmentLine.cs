using MagnaWms.Domain.Core.Exceptions;
using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.ShipmentAggregate;

public sealed class ShipmentLine : Entity
{
    private ShipmentLine() { }

    public ShipmentLine(long shipmentId, long itemId, decimal quantityShipped)
    {
        if (quantityShipped <= 0)
        {
            throw new DomainException("Quantity shipped must be positive.");
        }

        ShipmentId = shipmentId;
        ItemId = itemId;
        QuantityShipped = quantityShipped;
    }

    public long ShipmentId { get; private set; }
    public long ItemId { get; private set; }
    public decimal QuantityShipped { get; private set; }
}
