namespace MagnaWms.Contracts;

public sealed record ReceiptLineDto(
    long Id,
    long ItemId,
    string? ItemSku,
    string? ItemName,
    string? UnitOfMeasure,
    decimal ExpectedQty,
    decimal ReceivedQty,
    long? ToLocationId,
    string? Notes,
    bool IsFullyReceived
);
