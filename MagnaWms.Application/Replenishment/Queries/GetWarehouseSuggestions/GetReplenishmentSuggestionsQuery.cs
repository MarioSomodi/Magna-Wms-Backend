using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Replenishments;
using MediatR;

namespace MagnaWms.Application.Replenishment.Queries.GetWarehouseSuggestions;

public sealed record GetReplenishmentSuggestionsQuery(
    long WarehouseId,
    int HorizonDays
) : IRequest<Result<IReadOnlyList<ReplenishmentSuggestionDto>>>;
