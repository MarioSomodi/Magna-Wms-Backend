using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Warehouses.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.WarehouseAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Warehouses.Queries.GetAllWarehouses;

public sealed class GetAllWarehousesQueryHandler
    : IRequestHandler<GetAllWarehousesQuery, Result<IReadOnlyList<WarehouseDto>>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetAllWarehousesQueryHandler(IWarehouseRepository warehouseRepository, IMapper mapper, ICurrentUser currentUser)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<WarehouseDto>>> Handle(GetAllWarehousesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Warehouse> warehouses = _currentUser.IsSuperAdmin
            ? await _warehouseRepository.GetAllAsync(cancellationToken)
            : await _warehouseRepository.GetByIdsAsync(await _currentUser.GetAllowedWarehouses(), cancellationToken);

        if (warehouses.Count == 0)
        {
            return Result<IReadOnlyList<WarehouseDto>>.Failure(
                new Error(ErrorCode.NotFound, "No warehouses found."));
        }

        IReadOnlyList<WarehouseDto> dtoList = _mapper.Map<IReadOnlyList<WarehouseDto>>(warehouses);

        return Result<IReadOnlyList<WarehouseDto>>.Success(dtoList);
    }
}
