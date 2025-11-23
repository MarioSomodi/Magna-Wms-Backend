namespace MagnaWms.Contracts.Putaway;

public sealed record CreatePutawayTaskRequest(
    long ReceiptId,
    long ReceiptLineId,
    decimal QuantityToPutaway
);
