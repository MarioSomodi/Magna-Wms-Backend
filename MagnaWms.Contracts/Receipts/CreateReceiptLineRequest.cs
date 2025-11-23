namespace MagnaWms.Contracts.Receipts;
public sealed record CreateReceiptLineRequest(
    long ItemId,
    decimal ExpectedQty
);
