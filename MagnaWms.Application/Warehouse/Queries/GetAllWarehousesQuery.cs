using MediatR;
using MagnaWms.Contracts;

namespace MagnaWms.Application.Warehouse.Queries;

public sealed record GetAllWarehousesQuery : IRequest<IReadOnlyList<WarehouseDto>>;
