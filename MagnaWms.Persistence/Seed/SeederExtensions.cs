using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MagnaWms.Persistence.Seed;

public static class SeederExtensions
{
    public static IServiceCollection AddDevelopmentSeeder(this IServiceCollection services)
    {
        services.AddHostedService<DevelopmentDataSeeder>();
        return services;
    }
}
