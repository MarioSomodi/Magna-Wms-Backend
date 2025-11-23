using MagnaWms.Domain.Core.Exceptions;
using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.ShipmentAggregate;

public sealed class Shipment : AggregateRoot
{
    private readonly List<ShipmentLine> _lines = [];

    private Shipment() { }

    public Shipment(
        long warehouseId,
        long salesOrderId,
        string shipmentNumber,
        string carrier,
        string trackingNumber,
        long shippedByUserId)
    {
        if (string.IsNullOrWhiteSpace(shipmentNumber))
        {
            throw new DomainException("Shipment number cannot be empty.");
        }

        WarehouseId = warehouseId;
        SalesOrderId = salesOrderId;
        ShipmentNumber = shipmentNumber.Trim();
        Carrier = carrier.Trim();
        TrackingNumber = trackingNumber.Trim();
        ShippedByUserId = shippedByUserId;
        ShippedUtc = DateTime.UtcNow;
    }

    public long WarehouseId { get; private set; }
    public long SalesOrderId { get; private set; }

    public string ShipmentNumber { get; private set; } = null!;
    public string Carrier { get; private set; } = null!;
    public string TrackingNumber { get; private set; } = null!;

    public long ShippedByUserId { get; private set; }
    public DateTime ShippedUtc { get; private set; }

    public IReadOnlyCollection<ShipmentLine> Lines => _lines.AsReadOnly();

    public void AddLine(long itemId, decimal quantityShipped)
    {
        if (quantityShipped <= 0)
        {
            throw new DomainException("Quantity shipped must be positive.");
        }

        _lines.Add(new ShipmentLine(Id, itemId, quantityShipped));
    }
}
