namespace MagnaWms.Contracts;

public sealed record ReceiveReceiptLineRequest(
    decimal Quantity,
    long ToLocationId,
    string? Notes
);
