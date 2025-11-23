namespace MagnaWms.Contracts.Pick;

public sealed record PickTaskLineDto(
    long Id,
    long ItemId,
    long LocationId,
    decimal QuantityToPick,
    decimal QuantityPicked,
    bool IsCompleted
);
