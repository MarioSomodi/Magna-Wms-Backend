using MagnaWms.Domain.Core.Exceptions;
using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.ReceiptAggregate;

public sealed class ReceiptLine : Entity
{
    private ReceiptLine() { }

    public ReceiptLine(long receiptId, long itemId, decimal expectedQty)
    {
        ReceiptId = receiptId;
        ItemId = itemId;
        ExpectedQuantity = expectedQty;
        ReceivedQuantity = 0m;
    }

    public long ReceiptId { get; private set; }
    public long ItemId { get; private set; }
    public string? ItemSku { get; private set; }
    public string? ItemName { get; private set; }
    public string? UnitOfMeasure { get; private set; }

    public decimal ExpectedQuantity { get; private set; }
    public decimal ReceivedQuantity { get; private set; }

    public long? ToLocationId { get; private set; }
    public string? Notes { get; private set; }

    public bool IsFullyReceived => ReceivedQuantity >= ExpectedQuantity;

    public void Receive(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("Received quantity must be positive.");
        }

        ReceivedQuantity += quantity;
    }
}
