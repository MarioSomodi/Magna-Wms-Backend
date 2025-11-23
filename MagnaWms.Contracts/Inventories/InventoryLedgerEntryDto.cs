namespace MagnaWms.Contracts.Inventories;

public sealed record InventoryLedgerEntryDto(
    long Id,
    long WarehouseId,
    long LocationId,
    long ItemId,
    DateTime TimestampUtc,
    decimal QuantityChange,
    decimal ResultingQuantityOnHand,
    string MovementType,
    string? ReferenceType,
    string? ReferenceNumber
);
