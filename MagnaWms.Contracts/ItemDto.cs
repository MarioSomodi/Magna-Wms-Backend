namespace MagnaWms.Contracts;
public sealed record ItemDto(
    long ItemId,
    string Sku,
    string Name,
    long UnitOfMeasureId,
    bool IsActive,
    DateTime CreatedUtc,
    DateTime UpdatedUtc);
