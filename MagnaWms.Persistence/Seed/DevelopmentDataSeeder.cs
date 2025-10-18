using Bogus;
using MagnaWms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MagnaWms.Persistence.Seed;

public sealed class DevelopmentDataSeeder : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _env;
    private readonly ILogger<DevelopmentDataSeeder> _logger;
    private static readonly string[] sampleUnitsOfMesaure = ["pcs", "kg", "ltr", "box"];

    public DevelopmentDataSeeder(IServiceProvider serviceProvider, IHostEnvironment env, ILogger<DevelopmentDataSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _env = env;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_env.IsDevelopment())
        {
            _logger.LogInformation("Skipping data seed — not in Development environment.");
            return;
        }

        _logger.LogInformation("Starting development data seeding with Bogus...");

        using IServiceScope scope = _serviceProvider.CreateScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync(cancellationToken).ConfigureAwait(false); // ensure DB up to date

        await SeedWarehouseAsync(db, cancellationToken).ConfigureAwait(false);
        await SeedLocationsAsync(db, cancellationToken).ConfigureAwait(false);
        await SeedItemsAsync(db, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Development data seeding completed.");
    }

    private static async Task SeedWarehouseAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Warehouses.AnyAsync(ct).ConfigureAwait(false))
        {
            return;
        }

        db.Warehouses.Add(new Warehouse
        {
            Code = "ZAG01",
            Name = "Zagreb Central Distribution Center",
            TimeZone = "Europe/Zagreb",
            IsActive = true,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        });

        await db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private static async Task SeedLocationsAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Locations.AnyAsync(ct).ConfigureAwait(false))
        {
            return;
        }

        Warehouse warehouse = await db.Warehouses.FirstAsync(ct).ConfigureAwait(false);

        Location[] locations =
        [
            new Location
            {
                WarehouseID = warehouse.WarehouseID,
                Code = "STAGE-01",
                Type = LocationTypes.Stage,
                MaxQty = 500,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            },
            new Location
            {
                WarehouseID = warehouse.WarehouseID,
                Code = "RACK-A1",
                Type = LocationTypes.Rack,
                MaxQty = 1000,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            },
            new Location
            {
                WarehouseID = warehouse.WarehouseID,
                Code = "BIN-001",
                Type = LocationTypes.Bin,
                MaxQty = 250,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            }
        ];

        db.Locations.AddRange(locations);
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private static async Task SeedItemsAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Items.AnyAsync(ct).ConfigureAwait(false))
        {
            return;
        }

        Randomizer.Seed = new Random(8675309);

        Faker<Item> faker = new Faker<Item>("en")
            .RuleFor(i => i.Sku, f => $"SKU-{f.IndexFaker + 1:D3}")
            .RuleFor(i => i.Name, f => f.Commerce.ProductName())
            .RuleFor(i => i.BaseUom, f => f.PickRandom(sampleUnitsOfMesaure))
            .RuleFor(i => i.BaseUomFull, (f, i) => i.BaseUom switch
            {
                "pcs" => "pieces",
                "kg" => "kilograms",
                "ltr" => "liters",
                "box" => "boxes",
                _ => "units"
            })
            .RuleFor(i => i.StandardCost, f => Math.Round(f.Random.Decimal(5, 50), 2))
            .RuleFor(i => i.LeadTimeDays, f => f.Random.Int(3, 14))
            .RuleFor(i => i.ReorderPoint, f => f.Random.Int(10, 100))
            .RuleFor(i => i.IsActive, _ => true)
            .RuleFor(i => i.CreatedUtc, _ => DateTime.UtcNow)
            .RuleFor(i => i.UpdatedUtc, _ => DateTime.UtcNow);

        List<Item> items = faker.Generate(5);
        db.Items.AddRange(items);
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
