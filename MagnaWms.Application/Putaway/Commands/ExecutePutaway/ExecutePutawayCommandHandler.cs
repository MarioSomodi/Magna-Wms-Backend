using System.Globalization;
using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Inventories.Repository;
using MagnaWms.Application.InventoryLedgers.Repository;
using MagnaWms.Application.Locations.Repository;
using MagnaWms.Application.Putaway.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.InventoryAggregate;
using MagnaWms.Domain.LocationAggregate;
using MagnaWms.Domain.PutawayAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Putaway.Commands.ExecutePutaway;

public sealed class ExecutePutawayCommandHandler
    : IRequestHandler<ExecutePutawayCommand, Result<PutawayTaskDto>>
{
    private readonly IPutawayTaskRepository _putawayRepo;
    private readonly IInventoryRepository _inventoryRepo;
    private readonly IInventoryLedgerRepository _ledgerRepo;
    private readonly ILocationRepository _locationRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ExecutePutawayCommandHandler(
        IPutawayTaskRepository putawayRepo,
        IInventoryRepository inventoryRepo,
        IInventoryLedgerRepository ledgerRepo,
        ILocationRepository locationRepo,
        ICurrentUser currentUser,
        IUnitOfWork uow,
        IMapper mapper)
    {
        _putawayRepo = putawayRepo;
        _inventoryRepo = inventoryRepo;
        _ledgerRepo = ledgerRepo;
        _locationRepo = locationRepo;
        _currentUser = currentUser;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<PutawayTaskDto>> Handle(
        ExecutePutawayCommand request,
        CancellationToken cancellationToken)
    {
        PutawayTask? task = await _putawayRepo.GetByIdAsync(request.TaskId, cancellationToken);

        if (task is null)
        {
            return Result<PutawayTaskDto>.Failure(
                new Error(ErrorCode.NotFound, "Putaway task not found."));
        }

        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(task.WarehouseId))
        {
            return Result<PutawayTaskDto>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot execute putaway tasks for this warehouse."));
        }

        // Validate destination location
        Location? destLocation = await _locationRepo.GetByIdAsync(request.DestinationLocationId, cancellationToken);

        if (destLocation is null || destLocation.WarehouseId != task.WarehouseId)
        {
            return Result<PutawayTaskDto>.Failure(
                new Error(ErrorCode.BadRequest, "Invalid destination location."));
        }

        long warehouseId = task.WarehouseId;
        long itemId = task.ItemId;

        IReadOnlyList<Location> stagingLocations = await _locationRepo.GetStageLocationForWarehouse(warehouseId, cancellationToken);

        if (stagingLocations.Count <= 0)
        {
            return Result<PutawayTaskDto>.Failure(
                new Error(ErrorCode.BadRequest, "No staging location inside warehouse."));
        }

        long stagingLocationId = stagingLocations[0].Id;

        Inventory? stagingInventory =
            await _inventoryRepo.GetByKeyAsync(warehouseId, stagingLocationId, itemId, cancellationToken)
            ?? new Inventory(warehouseId, stagingLocationId, itemId, 0);

        if (stagingInventory.QuantityAvailable < request.Quantity)
        {
            return Result<PutawayTaskDto>.Failure(
                new Error(ErrorCode.BadRequest, "Not enough stock in staging to move."));
        }

        stagingInventory.ApplyDelta(-request.Quantity);

        Inventory? destInventory =
            await _inventoryRepo.GetByKeyAsync(warehouseId, request.DestinationLocationId, itemId, cancellationToken)
            ?? new Inventory(warehouseId, request.DestinationLocationId, itemId, 0);

        destInventory.ApplyDelta(request.Quantity);

        var ledgerOut = new InventoryLedgerEntry(
            warehouseId,
            stagingLocationId,
            itemId,
            -request.Quantity,
            stagingInventory.QuantityOnHand,
            "Putaway",
            "PutawayTask",
            task.Id.ToString(CultureInfo.InvariantCulture)
        );

        var ledgerIn = new InventoryLedgerEntry(
            warehouseId,
            request.DestinationLocationId,
            itemId,
            request.Quantity,
            destInventory.QuantityOnHand,
            "Putaway",
            "PutawayTask",
            task.Id.ToString(CultureInfo.InvariantCulture)
        );

        await _ledgerRepo.AddAsync(ledgerOut, cancellationToken);
        await _ledgerRepo.AddAsync(ledgerIn, cancellationToken);

        task.Complete(request.Quantity, _currentUser.UserId!.Value);

        await _uow.SaveChangesAsync(cancellationToken);

        return Result<PutawayTaskDto>.Success(_mapper.Map<PutawayTaskDto>(task));
    }
}
