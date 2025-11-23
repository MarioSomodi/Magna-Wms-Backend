using MagnaWms.Application.Authentication.Repository;
using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authorization;
using MagnaWms.Application.Inventories.Repository;
using MagnaWms.Application.InventoryLedgers.Repository;
using MagnaWms.Application.Items.Repository;
using MagnaWms.Application.Locations.Repository;
using MagnaWms.Application.Putaway.Repository;
using MagnaWms.Application.Receipts.Repository;
using MagnaWms.Application.Roles.Repository;
using MagnaWms.Application.UnitOfMeasures.Repository;
using MagnaWms.Application.Users.Repository;
using MagnaWms.Application.Warehouses.Repository;
using MagnaWms.Persistence;
using MagnaWms.Persistence.Context;
using MagnaWms.Persistence.Repositories;
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
                sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
            }));

        services.AddDevelopmentSeeder();

        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>))
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IWarehouseRepository, WarehouseRepository>()
                .AddScoped<IItemRepository, ItemRepository>()
                .AddScoped<ILocationRepository, LocationRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
                .AddScoped<IPermissionRepository, PermissionRepository>()
                .AddScoped<IRoleRepository, RoleRepository>()
                .AddScoped<IInventoryRepository, InventoryRepository>()
                .AddScoped<IInventoryLedgerRepository, InventoryLedgerRepository>()
                .AddScoped<IReceiptRepository, ReceiptRepository>()
                .AddScoped<IPutawayTaskRepository, PutawayTaskRepository>()
                .AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();

        return services;
    }
}
