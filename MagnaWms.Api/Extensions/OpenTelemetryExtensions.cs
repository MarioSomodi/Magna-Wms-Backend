using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MagnaWms.Api.Extensions;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetryStubs(this IServiceCollection services, string serviceName)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(r => r
                .AddService(
                    serviceName: serviceName, 
                    serviceVersion: typeof(OpenTelemetryExtensions).Assembly.GetName().Version?.ToString() ?? "1.0.0"))
            .WithTracing(b =>
            {
                b.AddAspNetCoreInstrumentation(o =>
                {
                    o.RecordException = true;
                    o.Filter = http => true;
                });
                b.AddHttpClientInstrumentation();
                b.AddSqlClientInstrumentation(o => o.RecordException = true);

                // add OTLP/Jaeger in Phase 5.
#if DEBUG
                // Dev-only Console exporter for spans
                b.AddConsoleExporter();
#endif
            });

        return services;
    }
}
