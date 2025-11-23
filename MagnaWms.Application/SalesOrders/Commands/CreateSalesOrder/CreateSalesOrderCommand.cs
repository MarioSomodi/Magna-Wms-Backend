using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Sales;
using MediatR;

namespace MagnaWms.Application.SalesOrders.Commands.CreateSalesOrder;

public sealed record CreateSalesOrderCommand(
    long WarehouseId,
    string OrderNumber,
    string CustomerName,
    IReadOnlyList<CreateSalesOrderLineRequest> Lines
) : IRequest<Result<SalesOrderDto>>;
