namespace MagnaWms.Application.Core.Abstractions.Forecasting;
public interface IForecastingService
{
    /// <summary>
    /// Given daily history (date, demand quantity), returns horizonDays future points.
    /// </summary>
    IReadOnlyList<(DateTime ForecastDateUtc, decimal Quantity)> ForecastDemand(
        IReadOnlyList<(DateTime DateUtc, decimal Quantity)> history,
        int horizonDays);
}
