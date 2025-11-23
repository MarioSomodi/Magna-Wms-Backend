using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Inventories.Repository;
using MagnaWms.Application.PickTasks.Repository;
using MagnaWms.Application.SalesOrders.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Pick;
using MagnaWms.Domain.InventoryAggregate;
using MagnaWms.Domain.ItemAggregate;
using MagnaWms.Domain.PickTaskAggregate;
using MagnaWms.Domain.SalesOrderAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.PickTasks.Commands.CreatePickTask;

public sealed class CreatePickTaskCommandHandler
    : IRequestHandler<CreatePickTaskCommand, Result<PickTaskDto>>
{
    private readonly ISalesOrderRepository _orders;
    private readonly IPickTaskRepository _tasks;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IInventoryRepository _inventoryRepository;

    public CreatePickTaskCommandHandler(
        ISalesOrderRepository orders,
        IPickTaskRepository tasks,
        ICurrentUser currentUser,
        IUnitOfWork uow,
        IMapper mapper,
        IInventoryRepository inventoryRepository)
    {
        _orders = orders;
        _tasks = tasks;
        _currentUser = currentUser;
        _uow = uow;
        _mapper = mapper;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<Result<PickTaskDto>> Handle(
        CreatePickTaskCommand request,
        CancellationToken cancellationToken)
    {
        SalesOrder? order = await _orders.GetWithLinesAsync(request.SalesOrderId, cancellationToken);

        if (order is null)
        {
            return Result<PickTaskDto>.Failure(
                new Error(ErrorCode.NotFound, "Sales order not found."));
        }

        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(order.WarehouseId))
        {
            return Result<PickTaskDto>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot create pick tasks for this warehouse."));
        }

        order.MarkPickingStarted();

        var task = new PickTask(
            order.WarehouseId,
        order.Id,
            _currentUser.UserId!.Value);

        foreach (SalesOrderLine line in order.Lines)
        {
            IReadOnlyList<Inventory> inv = await _inventoryRepository.GetByWarehouseAndItemAsync(order.WarehouseId, line.ItemId, cancellationToken);
            Inventory inventory = inv.OrderByDescending(i => i.QuantityAvailable).First();
            task.AddLine(
                line.ItemId,
                inventory.LocationId,
                line.QuantityAllocated);
        }

        await _tasks.AddAsync(task, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        PickTaskDto dto = _mapper.Map<PickTaskDto>(task);
        return Result<PickTaskDto>.Success(dto);
    }
}
