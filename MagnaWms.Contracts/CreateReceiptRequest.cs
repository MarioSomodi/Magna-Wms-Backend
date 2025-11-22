namespace MagnaWms.Contracts;

public sealed record CreateReceiptRequest(
    long WarehouseId,
    string ReceiptNumber,
    string? ExternalReference,
    DateTime? ExpectedArrivalDate,
    IReadOnlyList<CreateReceiptLineRequest> Lines
);
