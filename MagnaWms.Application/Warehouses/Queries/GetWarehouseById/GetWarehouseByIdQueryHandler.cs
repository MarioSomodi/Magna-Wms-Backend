using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Warehouses.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Warehouses;
using MagnaWms.Domain.WarehouseAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Warehouses.Queries.GetWarehouseById;

public sealed class GetWarehouseByIdQueryHandler
    : IRequestHandler<GetWarehouseByIdQuery, Result<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public GetWarehouseByIdQueryHandler(IWarehouseRepository warehouseRepository, IMapper mapper, ICurrentUser currentUser)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<Result<WarehouseDto>> Handle(
        GetWarehouseByIdQuery request,
        CancellationToken cancellationToken)
    {
        Warehouse? warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken);

        if (warehouse is null)
        {
            return Result<WarehouseDto>.Failure(
                new Error(ErrorCode.NotFound, $"Warehouse with ID {request.WarehouseId} was not found."));
        }

        IReadOnlyList<long> usersAllowedWarehouses = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if(!usersAllowedWarehouses.Contains(request.WarehouseId) && !_currentUser.IsSuperAdmin)
        {
            return Result<WarehouseDto>.Failure(
                    new Error(ErrorCode.Forbidden, "You are not allowed to access this warehouse."));
        }

        WarehouseDto dto = _mapper.Map<WarehouseDto>(warehouse);

        return Result<WarehouseDto>.Success(dto);
    }
}
