namespace MagnaWms.Domain.SalesOrderAggregate;

public enum SalesOrderStatus
{
    None = 0,
    Open = 1,
    Allocated = 2,
    Picking = 3,
    Shipped = 4,
    Cancelled = 5
}
