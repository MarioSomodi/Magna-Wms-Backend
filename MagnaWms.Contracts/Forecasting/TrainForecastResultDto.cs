namespace MagnaWms.Contracts;

public sealed record TrainForecastResultDto(
    long WarehouseId,
    int ItemsConsidered,
    int ItemsForecasted,
    int ItemsSkippedInsufficientHistory
);
