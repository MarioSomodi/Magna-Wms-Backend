namespace MagnaWms.Contracts.Sales;
public sealed record CreateSalesOrderRequest(
    long WarehouseId,
    string OrderNumber,
    string CustomerName,
    IReadOnlyList<CreateSalesOrderLineRequest> Lines
);
