using MagnaWms.Domain.Core.Exceptions;
using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.SalesOrderAggregate;

public sealed class SalesOrderLine : Entity
{
    private SalesOrderLine() { }

    public SalesOrderLine(long salesOrderId, long itemId, decimal quantityOrdered)
    {
        if (quantityOrdered <= 0)
        {
            throw new DomainException("Ordered quantity must be positive.");
        }

        SalesOrderId = salesOrderId;
        ItemId = itemId;
        QuantityOrdered = quantityOrdered;
        QuantityAllocated = 0m;
        QuantityPicked = 0m;
    }

    public long SalesOrderId { get; private set; }
    public long ItemId { get; private set; }

    public decimal QuantityOrdered { get; private set; }
    public decimal QuantityAllocated { get; private set; }
    public decimal QuantityPicked { get; private set; }

    public bool IsFullyAllocated => QuantityAllocated >= QuantityOrdered;
    public bool IsFullyPicked => QuantityPicked >= QuantityOrdered;

    public void Allocate(decimal qty)
    {
        if (qty <= 0)
        {
            throw new DomainException("Allocation quantity must be positive.");
        }

        decimal newAllocated = QuantityAllocated + qty;

        if (newAllocated > QuantityOrdered)
        {
            throw new DomainException(
                $"Cannot allocate {qty}. Would exceed ordered quantity {QuantityOrdered}.");
        }

        QuantityAllocated = newAllocated;
    }

    public void Pick(decimal qty)
    {
        if (qty <= 0)
        {
            throw new DomainException("Pick quantity must be positive.");
        }

        decimal newPicked = QuantityPicked + qty;

        if (newPicked > QuantityAllocated)
        {
            throw new DomainException(
                $"Cannot pick {qty}. Would exceed allocated quantity {QuantityAllocated}.");
        }

        QuantityPicked = newPicked;
    }
}
