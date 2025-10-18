using MagnaWms.Persistence.Context;
using MagnaWms.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Api.Configuration;

public static class PersistenceSetup
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        string connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }));

        services.AddDevelopmentSeeder();

        return services;
    }
}
