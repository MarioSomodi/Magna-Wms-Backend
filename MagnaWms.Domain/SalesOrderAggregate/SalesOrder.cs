using MagnaWms.Domain.Core.Exceptions;
using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.SalesOrderAggregate;

public sealed class SalesOrder : AggregateRoot
{
    private readonly List<SalesOrderLine> _lines = [];

    private SalesOrder() { }

    public SalesOrder(
        long warehouseId,
        string orderNumber,
        string customerName)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
        {
            throw new DomainException("Order number cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new DomainException("Customer name cannot be empty.");
        }

        WarehouseId = warehouseId;
        OrderNumber = orderNumber.Trim();
        CustomerName = customerName.Trim();
        Status = SalesOrderStatus.Open;
    }

    public long WarehouseId { get; private set; }
    public string OrderNumber { get; private set; } = null!;
    public string CustomerName { get; private set; } = null!;
    public SalesOrderStatus Status { get; private set; }

    public IReadOnlyCollection<SalesOrderLine> Lines => _lines.AsReadOnly();

    public void AddLine(long itemId, decimal quantityOrdered)
    {
        if (Status != SalesOrderStatus.Open)
        {
            throw new DomainException("Cannot add lines once the sales order is no longer open.");
        }

        if (quantityOrdered <= 0)
        {
            throw new DomainException("Ordered quantity must be positive.");
        }

        _lines.Add(new SalesOrderLine(Id, itemId, quantityOrdered));
    }

    public void AllocateLine(long lineId, decimal quantity)
    {
        EnsureStatusAllowsAllocation();

        SalesOrderLine line = GetLine(lineId);
        line.Allocate(quantity);

        if (_lines.All(l => l.IsFullyAllocated))
        {
            Status = SalesOrderStatus.Allocated;
        }
    }

    public void MarkPickingStarted()
    {
        if (Status is not SalesOrderStatus.Allocated and not SalesOrderStatus.Open)
        {
            throw new DomainException($"Cannot start picking when order is in status '{Status}'.");
        }

        Status = SalesOrderStatus.Picking;
    }

    public void RegisterPick(long lineId, decimal quantity)
    {
        if (Status is not SalesOrderStatus.Picking and not SalesOrderStatus.Allocated)
        {
            throw new DomainException($"Cannot pick items when order is in status '{Status}'.");
        }

        SalesOrderLine line = GetLine(lineId);
        line.Pick(quantity);
    }

    public void MarkShipped()
    {
        if (!_lines.TrueForAll(l => l.IsFullyPicked))
        {
            throw new DomainException("Cannot mark order as shipped; not all lines are fully picked.");
        }

        Status = SalesOrderStatus.Shipped;
    }

    public void Cancel()
    {
        if (Status is SalesOrderStatus.Shipped)
        {
            throw new DomainException("Cannot cancel a shipped order.");
        }

        Status = SalesOrderStatus.Cancelled;
    }

    private SalesOrderLine GetLine(long lineId) =>
        _lines.FirstOrDefault(l => l.Id == lineId)
        ?? throw new DomainException($"Sales order line {lineId} not found.");

    private void EnsureStatusAllowsAllocation()
    {
        if (Status is not SalesOrderStatus.Open and not SalesOrderStatus.Allocated)
        {
            throw new DomainException($"Cannot allocate in status '{Status}'.");
        }
    }
}
