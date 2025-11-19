using MagnaWms.Api.Configuration.Options;
using Microsoft.Extensions.Options;

namespace MagnaWms.Api.Configuration;

public static class CorsSetup
{
    private const string PolicyName = "MagnaCorsPolicy";

    public static IServiceCollection AddCorsSupport(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CorsOptions>(configuration.GetSection(CorsOptions.SectionName));

        services.AddCors(options => options.AddPolicy(PolicyName, builder =>
            {
                // Resolve allowed origins from configuration dynamically 
                using ServiceProvider serviceProvider = services.BuildServiceProvider();
                IOptionsMonitor<CorsOptions> monitor = serviceProvider.GetRequiredService<IOptionsMonitor<CorsOptions>>();
                string[] origins = monitor.CurrentValue.AllowedOrigins;

                if (origins.Length > 0)
                {
                    builder.WithOrigins(origins);
                }

                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            }));

        return services;
    }

    public static IApplicationBuilder UseCorsSupport(this IApplicationBuilder app)
    {
        app.UseCors(PolicyName);
        return app;
    }
}
