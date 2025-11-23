namespace MagnaWms.Contracts.Shippings;
public sealed record CreateShipmentRequest(
    long SalesOrderId,
    string ShipmentNumber,
    string Carrier,
    string TrackingNumber,
    IReadOnlyList<ShipmentLineRequest> Lines
);
