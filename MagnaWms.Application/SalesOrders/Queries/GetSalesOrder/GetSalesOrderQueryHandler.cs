using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.SalesOrders.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Sales;
using MagnaWms.Domain.SalesOrderAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.SalesOrders.Queries.GetSalesOrder;

public sealed class GetSalesOrderQueryHandler
    : IRequestHandler<GetSalesOrderQuery, Result<SalesOrderDto>>
{
    private readonly ISalesOrderRepository _repo;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetSalesOrderQueryHandler(
        ISalesOrderRepository repo,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _repo = repo;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<SalesOrderDto>> Handle(
        GetSalesOrderQuery request,
        CancellationToken cancellationToken)
    {
        SalesOrder? order = await _repo.GetWithLinesAsync(request.SalesOrderId, cancellationToken);

        if (order is null)
        {
            return Result<SalesOrderDto>.Failure(
                new Error(ErrorCode.NotFound, "Sales order not found."));
        }

        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(order.WarehouseId))
        {
            return Result<SalesOrderDto>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot access this order."));
        }

        SalesOrderDto dto = _mapper.Map<SalesOrderDto>(order);
        return Result<SalesOrderDto>.Success(dto);
    }
}
