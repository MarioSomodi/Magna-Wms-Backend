using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.SalesOrders.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Sales;
using MagnaWms.Domain.SalesOrderAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.SalesOrders.Commands.CreateSalesOrder;

public sealed class CreateSalesOrderCommandHandler
    : IRequestHandler<CreateSalesOrderCommand, Result<SalesOrderDto>>
{
    private readonly ISalesOrderRepository _repo;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateSalesOrderCommandHandler(
        ISalesOrderRepository repo,
        ICurrentUser currentUser,
        IUnitOfWork uow,
        IMapper mapper)
    {
        _repo = repo;
        _currentUser = currentUser;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<SalesOrderDto>> Handle(
        CreateSalesOrderCommand request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(request.WarehouseId))
        {
            return Result<SalesOrderDto>.Failure(
                new Error(ErrorCode.Forbidden,
                    "You are not allowed to create orders for this warehouse."));
        }

        var order = new SalesOrder(
            request.WarehouseId,
            request.OrderNumber,
            request.CustomerName);

        foreach (CreateSalesOrderLineRequest line in request.Lines)
        {
            order.AddLine(line.ItemId, line.QuantityOrdered);
        }

        await _repo.AddAsync(order, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        SalesOrderDto dto = _mapper.Map<SalesOrderDto>(order);
        return Result<SalesOrderDto>.Success(dto);
    }
}
