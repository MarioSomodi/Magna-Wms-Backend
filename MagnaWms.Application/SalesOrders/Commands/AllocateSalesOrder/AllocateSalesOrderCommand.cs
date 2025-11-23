using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Sales;
using MediatR;

namespace MagnaWms.Application.SalesOrders.Commands.AllocateSalesOrder;

public sealed record AllocateSalesOrderCommand(long SalesOrderId)
    : IRequest<Result<SalesOrderDto>>;
