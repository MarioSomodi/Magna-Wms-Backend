namespace MagnaWms.Contracts.Sales;
public sealed record CreateSalesOrderLineRequest(
    long ItemId,
    decimal QuantityOrdered
);
