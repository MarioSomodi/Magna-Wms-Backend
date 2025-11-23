using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Forecasting.Repository;
using MagnaWms.Application.Inventories.Repository;
using MagnaWms.Application.Items.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Replenishments;
using MagnaWms.Domain.ForecastAggregate;
using MagnaWms.Domain.InventoryAggregate;
using MagnaWms.Domain.ItemAggregate;
using MediatR;

namespace MagnaWms.Application.Replenishment.Queries.GetWarehouseSuggestions;

public sealed class GetReplenishmentSuggestionsQueryHandler
    : IRequestHandler<GetReplenishmentSuggestionsQuery, Result<IReadOnlyList<ReplenishmentSuggestionDto>>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IForecastSeriesRepository _forecastRepo;
    private readonly IInventoryRepository _inventoryRepo;
    private readonly IItemRepository _itemRepo;

    public GetReplenishmentSuggestionsQueryHandler(
        ICurrentUser currentUser,
        IForecastSeriesRepository forecastRepo,
        IInventoryRepository inventoryRepo,
        IItemRepository itemRepo)
    {
        _currentUser = currentUser;
        _forecastRepo = forecastRepo;
        _inventoryRepo = inventoryRepo;
        _itemRepo = itemRepo;
    }

    public async Task<Result<IReadOnlyList<ReplenishmentSuggestionDto>>> Handle(
        GetReplenishmentSuggestionsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);
        if (!_currentUser.IsSuperAdmin && !allowed.Contains(request.WarehouseId))
        {
            return Result<IReadOnlyList<ReplenishmentSuggestionDto>>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot access this warehouse."));
        }

        IReadOnlyList<Item> items = await _itemRepo.GetAllAsync(cancellationToken);
        IReadOnlyList<Inventory> inventory = await _inventoryRepo.GetByWarehousesAsync(new[] { request.WarehouseId }, cancellationToken);

        List<ReplenishmentSuggestionDto> results = [];

        foreach (Item item in items)
        {
            int leadTimeDays = item.LeadTimeDays ?? 14;

            Inventory? inv = inventory.FirstOrDefault(i => i.ItemId == item.Id);
            decimal qtyAvailable = inv?.QuantityAvailable ?? 0;

            ForecastSeries? series = await _forecastRepo.GetLatestForItemAsync(
                request.WarehouseId, item.Id, request.HorizonDays, cancellationToken);

            if (series is null)
            {
                continue;
            }

            decimal leadTimeDemand = series.Points
                .Where(p => p.ForecastDateUtc <= DateTime.UtcNow.AddDays(leadTimeDays))
                .Sum(p => p.Quantity);

            decimal safetyStock = leadTimeDemand * 0.20m;
            decimal reorderPoint = item.ReorderPoint ?? (leadTimeDemand + safetyStock);
            decimal suggested = Math.Max(0, reorderPoint - qtyAvailable);

            if (suggested <= 0)
            {
                continue;
            }

            results.Add(new ReplenishmentSuggestionDto(
                item.Id,
                qtyAvailable,
                leadTimeDemand,
                safetyStock,
                reorderPoint,
                suggested
            ));
        }

        return Result<IReadOnlyList<ReplenishmentSuggestionDto>>.Success(results);
    }
}
