namespace MagnaWms.Contracts.Receipts;

public sealed record ReceiptDto(
    long Id,
    long WarehouseId,
    string ReceiptNumber,
    string? ExternalReference,
    DateTime? ExpectedArrivalDate,
    string Status,
    DateTime CreatedUtc,
    DateTime? ClosedUtc,
    long CreatedByUserId,
    long? ReceivedByUserId,
    IReadOnlyList<ReceiptLineDto> Lines
);
