namespace MagnaWms.Contracts;

public sealed record InventoryDto(
    long Id,
    long WarehouseId,
    long LocationId,
    long ItemId,
    decimal QuantityOnHand,
    decimal QuantityAllocated,
    decimal QuantityAvailable
);
