using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.SalesOrders.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.SalesOrderAggregate;
using MediatR;

namespace MagnaWms.Application.SalesOrders.Commands.CancelSalesOrder;

public sealed class CancelSalesOrderCommandHandler
    : IRequestHandler<CancelSalesOrderCommand, Result<Success>>
{
    private readonly ISalesOrderRepository _repo;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _uow;

    public CancelSalesOrderCommandHandler(
        ISalesOrderRepository repo,
        ICurrentUser currentUser,
        IUnitOfWork uow)
    {
        _repo = repo;
        _currentUser = currentUser;
        _uow = uow;
    }

    public async Task<Result<Success>> Handle(
        CancelSalesOrderCommand request,
        CancellationToken cancellationToken)
    {
        SalesOrder? order = await _repo.GetWithLinesAsync(request.SalesOrderId, cancellationToken);

        if (order is null)
        {
            return Result<Success>.Failure(
                new Error(ErrorCode.NotFound, "Sales order not found."));
        }

        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(order.WarehouseId))
        {
            return Result<Success>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot cancel this order."));
        }

        try
        {
            order.Cancel();
        }
        catch (Exception ex)
        {
            return Result<Success>.Failure(
                new Error(ErrorCode.BadRequest, ex.Message));
        }

        await _uow.SaveChangesAsync(cancellationToken);
        return Result<Success>.Success(new Success("Canceled"));
    }
}
