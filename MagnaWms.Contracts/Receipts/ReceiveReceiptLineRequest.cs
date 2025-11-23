namespace MagnaWms.Contracts.Receipts;

public sealed record ReceiveReceiptLineRequest(
    decimal Quantity,
    long ToLocationId,
    string? Notes
);
