namespace MagnaWms.Contracts.Sales;

public sealed record SalesOrderDto(
    long Id,
    long WarehouseId,
    string OrderNumber,
    string CustomerName,
    string Status,
    DateTime CreatedUtc,
    DateTime UpdatedUtc,
    IReadOnlyList<SalesOrderLineDto> Lines
);
