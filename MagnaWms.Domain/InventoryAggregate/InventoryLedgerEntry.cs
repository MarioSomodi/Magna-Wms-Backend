using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.InventoryAggregate;

public sealed class InventoryLedgerEntry : AggregateRoot
{
    private InventoryLedgerEntry() { }

    public InventoryLedgerEntry(
        long warehouseId,
        long locationId,
        long itemId,
        decimal quantityChange,
        decimal resultingQuantityOnHand,
        string movementType,
        string? referenceType,
        string? referenceNumber,
        DateTime? timestampUtc = null)
    {
        WarehouseId = warehouseId;
        LocationId = locationId;
        ItemId = itemId;
        QuantityChange = quantityChange;
        ResultingQuantityOnHand = resultingQuantityOnHand;
        MovementType = movementType;
        ReferenceType = referenceType;
        ReferenceNumber = referenceNumber;
        TimestampUtc = timestampUtc ?? DateTime.UtcNow;
    }

    public long WarehouseId { get; private set; }
    public long LocationId { get; private set; }
    public long ItemId { get; private set; }

    public DateTime TimestampUtc { get; private set; }

    /// <summary>
    /// Positive for inbound, negative for outbound, zero for reallocations.
    /// </summary>
    public decimal QuantityChange { get; private set; }

    /// <summary>
    /// Snapshot of QuantityOnHand after applying this movement.
    /// </summary>
    public decimal ResultingQuantityOnHand { get; private set; }

    /// <summary>
    /// High-level movement type (Receiving, Putaway, Adjustment, Pick, Shipment, Replenishment, etc.)
    /// </summary>
    public string MovementType { get; private set; } = null!;

    /// <summary>
    /// Optional reference type (e.g., "PurchaseOrder", "SalesOrder", "Adjustment").
    /// </summary>
    public string? ReferenceType { get; private set; }

    /// <summary>
    /// Optional reference number / code (e.g., PO number, SO number).
    /// </summary>
    public string? ReferenceNumber { get; private set; }
}
