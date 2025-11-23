namespace MagnaWms.Contracts;

public sealed record CreatePutawayTaskRequest(
    long ReceiptId,
    long ReceiptLineId,
    decimal QuantityToPutaway
);
