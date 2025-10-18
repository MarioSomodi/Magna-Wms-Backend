using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MagnaWms.Api.Configuration.Options;

public sealed class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        => _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (ApiVersionDescription description in _provider.ApiVersionDescriptions)
        {
            var info = new OpenApiInfo
            {
                Title = "Magna WMS API",
                Version = description.GroupName,
                Description = "Warehouse Management System backend API for research and demonstration.",
                Contact = new OpenApiContact
                {
                    Name = "Mario Somodi",
                    Email = "mario.somodi@magna.wms",
                    Url = new Uri("https://github.com/MarioSomodi/Magna-Wms-Backend")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            };

            if (description.IsDeprecated)
            {
                info.Description += "⚠️ This API version is deprecated.";
            }
            options.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
            options.DocInclusionPredicate((version, desc) =>
            {
                IEnumerable<ApiVersionAttribute> values = desc.ActionDescriptor.EndpointMetadata.OfType<ApiVersionAttribute>();
                return values.Any(v => string.Equals($"v{v.Versions[0].MajorVersion}", version, StringComparison.Ordinal));
            });
            options.SwaggerDoc(description.GroupName, info);
        }
    }
}
