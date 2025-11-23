using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Inventories;
using MediatR;

namespace MagnaWms.Application.InventoryLedgers.Queries.GetInventoryLedger;

public sealed record GetInventoryLedgerQuery(
    long WarehouseId,
    long ItemId,
    long? LocationId)
    : IRequest<Result<IReadOnlyList<InventoryLedgerEntryDto>>>;
