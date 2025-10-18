using MediatR;
using MagnaWms.Persistence;
using MagnaWms.Contracts;
using System;

namespace MagnaWms.Application.Warehouse.Queries;

public sealed class GetAllWarehousesQueryHandler
    : IRequestHandler<GetAllWarehousesQuery, IReadOnlyList<WarehouseDto>>
{
    private readonly AppDbContext _db;

    public GetAllWarehousesQueryHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<WarehouseDto>> Handle(GetAllWarehousesQuery request, CancellationToken ct)
    {
        return await _db.Warehouses
            .AsNoTracking()
            .OrderBy(x => x.WarehouseID)
            .Select(x => new WarehouseDto(
                x.WarehouseID,
                x.Code,
                x.Name,
                x.TimeZone,
                x.IsActive))
            .ToListAsync(ct);
    }
}
