using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Inventories.Queries.GetInventory;

public sealed record GetInventoryQuery(long? WarehouseId)
    : IRequest<Result<IReadOnlyList<InventoryDto>>>;
