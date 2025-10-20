using MagnaWms.Application.Warehouses.Repository;
using MagnaWms.Contracts;
using MagnaWms.Domain.WarehouseAggregate;
using MediatR;

namespace MagnaWms.Application.Warehouses.Queries;

public sealed class GetAllWarehousesQueryHandler
    : IRequestHandler<GetAllWarehousesQuery, IReadOnlyList<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;

    public GetAllWarehousesQueryHandler(IWarehouseRepository warehouseRepository) => _warehouseRepository = warehouseRepository ?? throw new ArgumentNullException(nameof(warehouseRepository));

    public async Task<IReadOnlyList<WarehouseDto>> Handle(GetAllWarehousesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Warehouse> warehouses = await _warehouseRepository.GetAllAsync(cancellationToken);

        var result = warehouses
            .Select(w => new WarehouseDto(
                w.Id,
                w.Code,
                w.Name,
                w.TimeZone,
                w.IsActive))
            .ToList();

        return result;
    }
}
