using MediatR;
using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Warehouses;

namespace MagnaWms.Application.Warehouses.Queries.GetAllWarehouses;

public sealed record GetAllWarehousesQuery : IRequest<Result<IReadOnlyList<WarehouseDto>>>;
