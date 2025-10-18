using Asp.Versioning.ApiExplorer;

namespace MagnaWms.Api.Middleware;

public static class SwaggerMiddleware
{
    public static WebApplication UseSwaggerUIWithVersions(this WebApplication app)
    {
        IWebHostEnvironment env = app.Environment;
        if (!env.IsDevelopment())
        {
            return app;
        }

        IApiVersionDescriptionProvider provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    $"Magna WMS API {description.GroupName.ToUpperInvariant()}"
                );
            }

            options.DocumentTitle = "Magna WMS API Documentation";
            options.DisplayRequestDuration();
            options.EnableDeepLinking();
            options.EnableFilter();
        });

        app.MapGet("/", () => Results.Redirect("/swagger"));

        return app;
    }
}
