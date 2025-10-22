using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace MagnaWms.Application.Core.Mapping;

public static class MappingRegistration
{
    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;

        config.Scan(typeof(AssemblyMarker).Assembly);

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
