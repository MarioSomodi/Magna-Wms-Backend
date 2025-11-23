using System.Globalization;
using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Inventories.Repository;
using MagnaWms.Application.InventoryLedgers.Repository;
using MagnaWms.Application.PickTasks.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Pick;
using MagnaWms.Domain.InventoryAggregate;
using MagnaWms.Domain.PickTaskAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.PickTasks.Commands.PickItem;

public sealed class PickItemCommandHandler
    : IRequestHandler<PickItemCommand, Result<PickTaskDto>>
{
    private readonly IPickTaskRepository _tasks;
    private readonly IInventoryRepository _inventory;
    private readonly IInventoryLedgerRepository _ledger;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public PickItemCommandHandler(
        IPickTaskRepository tasks,
        IInventoryRepository inventory,
        IInventoryLedgerRepository ledger,
        ICurrentUser currentUser,
        IUnitOfWork uow,
        IMapper mapper)
    {
        _tasks = tasks;
        _inventory = inventory;
        _ledger = ledger;
        _currentUser = currentUser;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<PickTaskDto>> Handle(
        PickItemCommand request,
        CancellationToken cancellationToken)
    {
        PickTask? task = await _tasks.GetWithLinesAsync(request.TaskId, cancellationToken);

        if (task is null)
        {
            return Result<PickTaskDto>.Failure(
                new Error(ErrorCode.NotFound, "Pick task not found."));
        }

        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(task.WarehouseId))
        {
            return Result<PickTaskDto>.Failure(
                new Error(ErrorCode.Forbidden, "Not allowed."));
        }

        PickTaskLine line = task.Lines.First(l => l.Id == request.LineId);

        Inventory? inv = await _inventory.GetByKeyAsync(
            task.WarehouseId,
            line.LocationId,
            line.ItemId,
            cancellationToken);

        if (inv is null || inv.QuantityAvailable < request.QuantityPicked)
        {
            return Result<PickTaskDto>.Failure(
                new Error(ErrorCode.BadRequest, "Not enough inventory at this location."));
        }

        inv.ApplyDelta(-request.QuantityPicked);

        var entry = new InventoryLedgerEntry(
            task.WarehouseId,
            line.LocationId,
            line.ItemId,
            -request.QuantityPicked,
            inv.QuantityOnHand,
            "Pick",
            "PickTask",
            task.Id.ToString(CultureInfo.InvariantCulture)
        );

        await _ledger.AddAsync(entry, cancellationToken);

        task.RegisterPick(request.LineId, request.QuantityPicked, _currentUser.UserId!.Value);

        await _uow.SaveChangesAsync(cancellationToken);

        return Result<PickTaskDto>.Success(_mapper.Map<PickTaskDto>(task));
    }
}
