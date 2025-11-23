using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Sales;
using MediatR;

namespace MagnaWms.Application.SalesOrders.Queries.GetSalesOrders;

public sealed record GetSalesOrdersQuery(long WarehouseId)
    : IRequest<Result<IReadOnlyList<SalesOrderDto>>>;
