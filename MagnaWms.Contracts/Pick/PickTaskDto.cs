namespace MagnaWms.Contracts.Pick;

public sealed record PickTaskDto(
    long Id,
    long WarehouseId,
    long SalesOrderId,
    string Status,
    DateTime CreatedUtc,
    DateTime UpdatedUtc,
    long CreatedByUserId,
    long? CompletedByUserId,
    DateTime? CompletedUtc,
    IReadOnlyList<PickTaskLineDto> Lines
);
