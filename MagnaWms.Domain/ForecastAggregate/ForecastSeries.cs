using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.ForecastAggregate;

public sealed class ForecastSeries : AggregateRoot
{
    private readonly List<ForecastSeriesPoint> _points = [];

    private ForecastSeries() { }

    public ForecastSeries(
        long warehouseId,
        long itemId,
        int horizonDays,
        string modelInfo)
    {
        WarehouseId = warehouseId;
        ItemId = itemId;
        HorizonDays = horizonDays;
        ModelInfo = modelInfo;
    }

    public long WarehouseId { get; private set; }
    public long ItemId { get; private set; }

    /// <summary>
    /// Forecast horizon in days.
    /// </summary>
    public int HorizonDays { get; private set; }

    /// <summary>
    /// Model identifier / description (e.g. "SSA_v1").
    /// </summary>
    public string ModelInfo { get; private set; } = null!;

    public IReadOnlyCollection<ForecastSeriesPoint> Points => _points.AsReadOnly();

    public void AddPoint(DateTime forecastDateUtc, decimal quantity) => _points.Add(new ForecastSeriesPoint(Id, forecastDateUtc, quantity));

    public void ClearPoints() => _points.Clear();
}
