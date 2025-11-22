using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Inventories.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Inventories.Queries.GetInventory;

public sealed class GetInventoryQueryHandler
    : IRequestHandler<GetInventoryQuery, Result<IReadOnlyList<InventoryDto>>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetInventoryQueryHandler(
        IInventoryRepository inventoryRepository,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _inventoryRepository = inventoryRepository;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<InventoryDto>>> Handle(
        GetInventoryQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<long> allowedWarehouses = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && allowedWarehouses.Count == 0)
        {
            return Result<IReadOnlyList<InventoryDto>>.Failure(
                new Error(ErrorCode.Forbidden, "You are not allowed to access any warehouses."));
        }

        IReadOnlyList<Domain.InventoryAggregate.Inventory> inventory;

        if (request.WarehouseId is not null)
        {
            long warehouseId = request.WarehouseId.Value;

            if (!_currentUser.IsSuperAdmin && !allowedWarehouses.Contains(warehouseId))
            {
                return Result<IReadOnlyList<InventoryDto>>.Failure(
                    new Error(ErrorCode.Forbidden, "You are not allowed to access this warehouse."));
            }

            inventory = await _inventoryRepository.GetByWarehouseAsync(warehouseId, cancellationToken);
        }
        else
        {
            inventory = await _inventoryRepository.GetByWarehousesAsync(
                allowedWarehouses,
                cancellationToken);
        }

        IReadOnlyList<InventoryDto> dtos = _mapper.Map<IReadOnlyList<InventoryDto>>(inventory);

        return Result<IReadOnlyList<InventoryDto>>.Success(dtos);
    }
}
