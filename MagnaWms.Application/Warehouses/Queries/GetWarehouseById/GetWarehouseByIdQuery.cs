using MediatR;
using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Warehouses;

namespace MagnaWms.Application.Warehouses.Queries.GetWarehouseById;

public sealed record GetWarehouseByIdQuery(long WarehouseId) : IRequest<Result<WarehouseDto>>;
