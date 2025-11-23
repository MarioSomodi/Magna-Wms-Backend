namespace MagnaWms.Contracts;

public sealed record ForecastPointDto(
    DateTime ForecastDateUtc,
    decimal Quantity
);
