namespace MagnaWms.Contracts.Shippings;

public sealed record ShipmentLineDto(
    long Id,
    long ItemId,
    decimal QuantityShipped
);
