using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.SalesOrders.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Sales;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.SalesOrders.Queries.GetSalesOrders;

public sealed class GetSalesOrdersQueryHandler
    : IRequestHandler<GetSalesOrdersQuery, Result<IReadOnlyList<SalesOrderDto>>>
{
    private readonly ISalesOrderRepository _repo;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetSalesOrdersQueryHandler(
        ISalesOrderRepository repo,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _repo = repo;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<SalesOrderDto>>> Handle(
        GetSalesOrdersQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(request.WarehouseId))
        {
            return Result<IReadOnlyList<SalesOrderDto>>.Failure(
                new Error(ErrorCode.Forbidden,
                    "You cannot access orders for this warehouse."));
        }

        IReadOnlyList<Domain.SalesOrderAggregate.SalesOrder> orders =
            await _repo.GetByWarehouseAsync(request.WarehouseId, cancellationToken);

        var dtos = orders
            .Select(o => _mapper.Map<SalesOrderDto>(o))
            .ToList();

        return Result<IReadOnlyList<SalesOrderDto>>.Success(dtos);
    }
}
