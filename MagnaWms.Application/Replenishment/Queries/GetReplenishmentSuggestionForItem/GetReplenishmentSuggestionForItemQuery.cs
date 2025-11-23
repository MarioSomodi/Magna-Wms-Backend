using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Replenishments;
using MediatR;

namespace MagnaWms.Application.Replenishment.Queries.GetReplenishmentSuggestionForItem;

public sealed record GetReplenishmentSuggestionForItemQuery(
    long WarehouseId,
    long ItemId,
    int HorizonDays
) : IRequest<Result<ReplenishmentSuggestionDto>>;
