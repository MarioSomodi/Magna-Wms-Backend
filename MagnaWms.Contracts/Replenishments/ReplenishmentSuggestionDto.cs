namespace MagnaWms.Contracts.Replenishments;

public sealed record ReplenishmentSuggestionDto(
    long ItemId,
    decimal QuantityAvailable,
    decimal ForecastQtyLeadTime,
    decimal SafetyStock,
    decimal ReorderPoint,
    decimal SuggestedOrderQty
);
