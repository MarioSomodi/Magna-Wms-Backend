using MagnaWms.Application.Core.Abstractions.Forecasting;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace MagnaWms.Forecasting;

internal sealed class SsaForecastingService : IForecastingService
{
    private const int MinHistoryPoints = 14;
    private const int DefaultWindowSize = 7;
    private const int DefaultSeriesLength = 90;

    public IReadOnlyList<(DateTime ForecastDateUtc, decimal Quantity)> ForecastDemand(
        IReadOnlyList<(DateTime DateUtc, decimal Quantity)> history,
        int horizonDays)
    {
        if (history is null || history.Count < MinHistoryPoints || horizonDays <= 0)
        {
            return Array.Empty<(DateTime ForecastDateUtc, decimal Quantity)>();
        }

        List<(DateTime DateUtc, decimal Quantity)> ordered = history
            .Select(h => (h.DateUtc.Date, h.Quantity))
            .OrderBy(h => h.Date)
            .ToList();

        var rows = ordered
            .Select(h => new ModelInput
            {
                Quantity = (float)h.Quantity
            })
            .ToList();

        var ml = new MLContext();

        IDataView dataView = ml.Data.LoadFromEnumerable(rows);
        int trainSize = rows.Count;

        int windowSize = Math.Min(DefaultWindowSize, Math.Max(2, trainSize / 4));
        int seriesLength = Math.Max(DefaultSeriesLength, trainSize);

        SsaForecastingEstimator pipeline = ml.Forecasting.ForecastBySsa(
            outputColumnName: nameof(ModelOutput.ForecastedQuantity),
            inputColumnName: nameof(ModelInput.Quantity),
            windowSize: windowSize,
            seriesLength: seriesLength,
            trainSize: trainSize,
            horizon: horizonDays);

        ITransformer model = pipeline.Fit(dataView);

        var result = new List<(DateTime ForecastDateUtc, decimal Quantity)>();

        using (TimeSeriesPredictionEngine<ModelInput, ModelOutput> engine = model.CreateTimeSeriesEngine<ModelInput, ModelOutput>(ml))
        { 
            ModelOutput prediction = engine.Predict();

            DateTime lastDate = ordered[^1].DateUtc;
            int count = Math.Min(horizonDays, prediction.ForecastedQuantity.Length);

            for (int i = 0; i < count; i++)
            {
                DateTime forecastDate = lastDate.AddDays(i + 1);
                decimal qty = (decimal)prediction.ForecastedQuantity[i];

                if (qty < 0)
                {
                    qty = 0;
                }

                result.Add((forecastDate, qty));
            }
        }


        return result;
    }

    private sealed class ModelInput
    {
        public float Quantity { get; set; }
    }

    private sealed class ModelOutput
    {
        public float[] ForecastedQuantity { get; set; } = Array.Empty<float>();
    }
}
