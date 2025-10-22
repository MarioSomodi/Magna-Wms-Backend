using MediatR;
using MagnaWms.Contracts;
using MagnaWms.Application.Core.Results;

namespace MagnaWms.Application.Warehouses.Queries.GetAllWarehouses;

public sealed record GetAllWarehousesQuery : IRequest<Result<IReadOnlyList<WarehouseDto>>>;
