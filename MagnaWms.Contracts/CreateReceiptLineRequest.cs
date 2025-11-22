namespace MagnaWms.Contracts;
public sealed record CreateReceiptLineRequest(
    long ItemId,
    decimal ExpectedQty
);
