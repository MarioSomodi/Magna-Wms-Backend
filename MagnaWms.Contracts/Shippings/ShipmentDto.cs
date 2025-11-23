namespace MagnaWms.Contracts.Shippings;

public sealed record ShipmentDto(
    long Id,
    long WarehouseId,
    long SalesOrderId,
    string ShipmentNumber,
    string Carrier,
    string TrackingNumber,
    long ShippedByUserId,
    DateTime ShippedUtc,
    IReadOnlyList<ShipmentLineDto> Lines
);
