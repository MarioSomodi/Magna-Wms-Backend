namespace MagnaWms.Contracts.UnitOfMeasures;
public sealed record UnitOfMeasureDto(
    long UnitOfMeasureId,
    string Symbol,
    string Name,
    DateTime CreatedUtc,
    DateTime UpdatedUtc);
