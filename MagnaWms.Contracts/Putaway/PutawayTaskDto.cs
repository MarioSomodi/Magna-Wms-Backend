namespace MagnaWms.Contracts.Putaway;

public sealed record PutawayTaskDto(
    long Id,
    long WarehouseId,
    long ReceiptId,
    long ReceiptLineId,
    long ItemId,
    decimal QuantityToPutaway,
    decimal QuantityCompleted,
    string Status,
    long CreatedByUserId,
    long? CompletedByUserId,
    DateTime CreatedUtc,
    DateTime? CompletedUtc
);
