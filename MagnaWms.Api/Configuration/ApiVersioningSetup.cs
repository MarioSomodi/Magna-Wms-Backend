using Asp.Versioning;

namespace MagnaWms.Api.Configuration;

public static class ApiVersioningSetup
{
    public static IServiceCollection AddApiVersioningWithExplorer(this IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = false;
                // adds response headers: api-supported-versions
                options.ReportApiVersions = true;
            })
            // enables attribute-based versioning for controllers
            .AddMvc()
            .AddApiExplorer(options =>
            {
                // configures grouping for Swagger
                options.GroupNameFormat = "'v'VVV"; // v1, v1.1, v2
                options.SubstituteApiVersionInUrl = true; // /api/v1/...
            });

        return services;
    }
}
