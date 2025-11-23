namespace MagnaWms.Contracts.Items;
public sealed record ItemDto(
    long ItemId,
    string Sku,
    string Name,
    long UnitOfMeasureId,
    bool IsActive,
    DateTime CreatedUtc,
    DateTime UpdatedUtc);
