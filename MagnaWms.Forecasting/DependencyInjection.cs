using MagnaWms.Application.Core.Abstractions.Forecasting;
using Microsoft.Extensions.DependencyInjection;

namespace MagnaWms.Forecasting;
public static class DependencyInjection
{
    public static IServiceCollection AddForecasting(this IServiceCollection services)
    {
        services.AddScoped<IForecastingService, SsaForecastingService>();
        return services;
    }
}
