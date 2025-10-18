using MagnaWms.Persistence.Seed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MagnaWms.Persistence.Extensions;

public static class SeederExtensions
{
    public static IServiceCollection AddDevelopmentSeeder(this IServiceCollection services)
    {
        services.AddHostedService<DevelopmentDataSeeder>();
        return services;
    }
}
