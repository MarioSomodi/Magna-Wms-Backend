using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Warehouses.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.WarehouseAggregate;
using MediatR;

namespace MagnaWms.Application.Warehouses.Queries.GetWarehouseById;

public sealed class GetWarehouseByIdQueryHandler
    : IRequestHandler<GetWarehouseByIdQuery, Result<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;

    public GetWarehouseByIdQueryHandler(IWarehouseRepository warehouseRepository) => _warehouseRepository = warehouseRepository;

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

        var dto = new WarehouseDto(
            warehouse.Id,
            warehouse.Code,
            warehouse.Name,
            warehouse.TimeZone,
            warehouse.IsActive);

        return Result<WarehouseDto>.Success(dto);
    }
}
