using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Forecasting.Repository;
using MagnaWms.Application.Inventories.Repository;
using MagnaWms.Application.Items.Repository;
using MagnaWms.Application.Replenishment.Queries.GetReplenishmentSuggestionForItem;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Replenishments;
using MagnaWms.Domain.ForecastAggregate;
using MagnaWms.Domain.InventoryAggregate;
using MagnaWms.Domain.ItemAggregate;
using MediatR;

namespace MagnaWms.Application.Replenishment.Queries.GetItemSuggestion;

public sealed class GetReplenishmentSuggestionForItemQueryHandler
    : IRequestHandler<GetReplenishmentSuggestionForItemQuery, Result<ReplenishmentSuggestionDto>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IInventoryRepository _inventoryRepo;
    private readonly IForecastSeriesRepository _forecastRepo;
    private readonly IItemRepository _itemRepo;

    public GetReplenishmentSuggestionForItemQueryHandler(
        ICurrentUser currentUser,
        IInventoryRepository inventoryRepo,
        IForecastSeriesRepository forecastRepo,
        IItemRepository itemRepo)
    {
        _currentUser = currentUser;
        _inventoryRepo = inventoryRepo;
        _forecastRepo = forecastRepo;
        _itemRepo = itemRepo;
    }

    public async Task<Result<ReplenishmentSuggestionDto>> Handle(
        GetReplenishmentSuggestionForItemQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);
        if (!_currentUser.IsSuperAdmin && !allowed.Contains(request.WarehouseId))
        {
            return Result<ReplenishmentSuggestionDto>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot access this warehouse."));
        }

        Item? item = await _itemRepo.GetByIdAsync(request.ItemId, cancellationToken);
        if (item is null)
        {
            return Result<ReplenishmentSuggestionDto>.Failure(
                new Error(ErrorCode.NotFound, "Item not found."));
        }

        int leadTimeDays = item.LeadTimeDays ?? 14;

        IReadOnlyList<Inventory> invList = await _inventoryRepo.GetByWarehousesAsync(new[] { request.WarehouseId }, cancellationToken);
        Inventory? inv = invList.FirstOrDefault(i => i.ItemId == request.ItemId);

        decimal qtyAvailable = inv?.QuantityAvailable ?? 0;

        ForecastSeries? series = await _forecastRepo.GetLatestForItemAsync(
            request.WarehouseId, request.ItemId, request.HorizonDays, cancellationToken);

        if (series is null)
        {
            return Result<ReplenishmentSuggestionDto>.Failure(
                new Error(ErrorCode.NotFound, "No forecast available for this item."));
        }

        decimal leadTimeDemand = series.Points
            .Where(p => p.ForecastDateUtc <= DateTime.UtcNow.AddDays(leadTimeDays))
            .Sum(p => p.Quantity);

        decimal safetyStock = leadTimeDemand * 0.20m;

        decimal reorderPoint = item.ReorderPoint ?? (leadTimeDemand + safetyStock);

        decimal suggested = Math.Max(0, reorderPoint - qtyAvailable);

        var dto = new ReplenishmentSuggestionDto(
            item.Id,
            qtyAvailable,
            leadTimeDemand,
            safetyStock,
            reorderPoint,
            suggested
        );

        return Result<ReplenishmentSuggestionDto>.Success(dto);
    }
}
