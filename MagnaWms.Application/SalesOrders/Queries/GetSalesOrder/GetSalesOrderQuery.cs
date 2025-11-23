using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Sales;
using MediatR;

namespace MagnaWms.Application.SalesOrders.Queries.GetSalesOrder;

public sealed record GetSalesOrderQuery(long SalesOrderId)
    : IRequest<Result<SalesOrderDto>>;
