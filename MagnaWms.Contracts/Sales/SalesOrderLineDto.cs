namespace MagnaWms.Contracts.Sales;

public sealed record SalesOrderLineDto(
    long Id,
    long ItemId,
    decimal QuantityOrdered,
    decimal QuantityAllocated,
    decimal QuantityPicked,
    bool IsFullyAllocated,
    bool IsFullyPicked
);
