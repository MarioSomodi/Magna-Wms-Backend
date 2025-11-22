using MagnaWms.Domain.Core.Exceptions;
using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.InventoryAggregate;

public sealed class Inventory : AggregateRoot
{
    private Inventory() { }

    public Inventory(long warehouseId, long locationId, long itemId, decimal quantityOnHand = 0m)
    {
        SetWarehouse(warehouseId);
        SetLocation(locationId);
        SetItem(itemId);
        SetQuantityOnHand(quantityOnHand);
    }

    public long WarehouseId { get; private set; }
    public long LocationId { get; private set; }
    public long ItemId { get; private set; }

    public decimal QuantityOnHand { get; private set; }

    public decimal QuantityAllocated { get; private set; }

    public decimal QuantityAvailable => QuantityOnHand - QuantityAllocated;

    public void ApplyDelta(decimal delta)
    {
        decimal newQty = QuantityOnHand + delta;

        if (newQty < 0)
        {
            throw new DomainException(
                $"Inventory cannot go negative. Attempted new quantity: {newQty} for Item {ItemId} at Location {LocationId}, Warehouse {WarehouseId}.");
        }

        QuantityOnHand = newQty;
    }

    public void Allocate(decimal qty)
    {
        if (qty <= 0)
        {
            throw new DomainException("Allocation quantity must be positive.");
        }

        if (QuantityAvailable < qty)
        {
            throw new DomainException(
                $"Cannot allocate {qty} units. Only {QuantityAvailable} available for Item {ItemId} at Location {LocationId}, Warehouse {WarehouseId}.");
        }

        QuantityAllocated += qty;
    }

    public void ReleaseAllocation(decimal qty)
    {
        if (qty <= 0)
        {
            throw new DomainException("Released allocation quantity must be positive.");
        }

        if (QuantityAllocated < qty)
        {
            throw new DomainException(
                $"Cannot release {qty} allocated units. Only {QuantityAllocated} allocated for Item {ItemId} at Location {LocationId}, Warehouse {WarehouseId}.");
        }

        QuantityAllocated -= qty;
    }

    private void SetWarehouse(long warehouseId)
    {
        if (warehouseId <= 0)
        {
            throw new DomainException("WarehouseId must be greater than zero.");
        }

        WarehouseId = warehouseId;
    }

    private void SetLocation(long locationId)
    {
        if (locationId <= 0)
        {
            throw new DomainException("LocationId must be greater than zero.");
        }

        LocationId = locationId;
    }

    private void SetItem(long itemId)
    {
        if (itemId <= 0)
        {
            throw new DomainException("ItemId must be greater than zero.");
        }

        ItemId = itemId;
    }

    private void SetQuantityOnHand(decimal qty)
    {
        if (qty < 0)
        {
            throw new DomainException("QuantityOnHand cannot be negative.");
        }

        QuantityOnHand = qty;
    }
}
