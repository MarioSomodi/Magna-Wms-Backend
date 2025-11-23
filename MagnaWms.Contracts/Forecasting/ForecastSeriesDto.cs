namespace MagnaWms.Contracts;

public sealed record ForecastSeriesDto(
    long WarehouseId,
    long ItemId,
    DateTime GeneratedUtc,
    int HorizonDays,
    string ModelInfo,
    IReadOnlyList<ForecastPointDto> Points
);
