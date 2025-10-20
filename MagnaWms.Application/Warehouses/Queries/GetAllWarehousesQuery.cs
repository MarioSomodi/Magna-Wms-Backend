using MediatR;
using MagnaWms.Contracts;

namespace MagnaWms.Application.Warehouses.Queries;

public sealed record GetAllWarehousesQuery : IRequest<IReadOnlyList<WarehouseDto>>;
