using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Warehouses.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.WarehouseAggregate;
using MediatR;

namespace MagnaWms.Application.Warehouses.Queries.GetAllWarehouses;

public sealed class GetAllWarehousesQueryHandler
    : IRequestHandler<GetAllWarehousesQuery, Result<IReadOnlyList<WarehouseDto>>>
{
    private readonly IWarehouseRepository _warehouseRepository;

    public GetAllWarehousesQueryHandler(IWarehouseRepository warehouseRepository) => _warehouseRepository = warehouseRepository;

    public async Task<Result<IReadOnlyList<WarehouseDto>>> Handle(GetAllWarehousesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Warehouse> warehouses = await _warehouseRepository.GetAllAsync(cancellationToken);

        if (warehouses.Count == 0)
        {
            return Result<IReadOnlyList<WarehouseDto>>.Failure(
                new Error(ErrorCode.NotFound, "No warehouses found."));
        }

        var dtoList = warehouses
            .Select(w => new WarehouseDto(
                w.Id,
                w.Code,
                w.Name,
                w.TimeZone,
                w.IsActive))
            .ToList();

        return Result<IReadOnlyList<WarehouseDto>>.Success(dtoList);
    }
}
