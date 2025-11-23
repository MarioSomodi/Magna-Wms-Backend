using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.SalesOrders.Commands.CancelSalesOrder;

public sealed record CancelSalesOrderCommand(long SalesOrderId)
    : IRequest<Result<Success>>;
