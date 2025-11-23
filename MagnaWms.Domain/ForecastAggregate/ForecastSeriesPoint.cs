using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.ForecastAggregate;

public sealed class ForecastSeriesPoint : Entity
{
    private ForecastSeriesPoint() { }

    public ForecastSeriesPoint(long forecastSeriesId, DateTime forecastDateUtc, decimal quantity)
    {
        ForecastSeriesId = forecastSeriesId;
        ForecastDateUtc = forecastDateUtc;
        Quantity = quantity;
    }

    public long ForecastSeriesId { get; private set; }

    /// <summary>
    /// Date (UTC) this forecast point refers to — day granularity.
    /// </summary>
    public DateTime ForecastDateUtc { get; private set; }

    /// <summary>
    /// Forecasted demand quantity for that date (units).
    /// </summary>
    public decimal Quantity { get; private set; }
}
