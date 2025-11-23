using MagnaWms.Domain.Core.Exceptions;
using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.PutawayAggregate;

public sealed class PutawayTask : AggregateRoot
{
    private PutawayTask() { }

    public PutawayTask(
        long warehouseId,
        long receiptId,
        long receiptLineId,
        long itemId,
        decimal quantityToPutaway,
        long createdByUserId)
    {
        if (quantityToPutaway <= 0)
        {
            throw new DomainException("Putaway quantity must be positive.");
        }

        WarehouseId = warehouseId;
        ReceiptId = receiptId;
        ReceiptLineId = receiptLineId;
        ItemId = itemId;
        QuantityToPutaway = quantityToPutaway;
        CreatedByUserId = createdByUserId;

        Status = OCStatus.Open;
    }

    public long WarehouseId { get; private set; }
    public long ReceiptId { get; private set; }
    public long ReceiptLineId { get; private set; }
    public long ItemId { get; private set; }

    public decimal QuantityToPutaway { get; private set; }
    public decimal QuantityCompleted { get; private set; }

    public OCStatus Status { get; private set; }

    public long CreatedByUserId { get; private set; }
    public long? CompletedByUserId { get; private set; }
    public DateTime? CompletedUtc { get; private set; }

    public bool IsCompleted => QuantityCompleted >= QuantityToPutaway;

    public void Complete(decimal qty, long userId)
    {
        if (qty <= 0)
        {
            throw new DomainException("Putaway move quantity must be positive.");
        }

        QuantityCompleted += qty;

        if (IsCompleted)
        {
            Status = OCStatus.Completed;
            CompletedByUserId = userId;
            CompletedUtc = DateTime.UtcNow;
        }
    }
}
