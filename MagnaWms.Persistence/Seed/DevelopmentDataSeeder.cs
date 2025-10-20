using Bogus;
using MagnaWms.Domain.ItemAggregate;
using MagnaWms.Domain.LocationAggregate;
using MagnaWms.Domain.UnitOfMeasureAggregate;
using MagnaWms.Domain.WarehouseAggregate;
using MagnaWms.Persistence.Context;
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

        await db.Database.MigrateAsync(cancellationToken); // ensure DB up to date

        await SeedWarehouseAsync(db, cancellationToken);
        await SeedLocationsAsync(db, cancellationToken);
        await SeedUnitOfMeasuresAsync(db, cancellationToken);
        await SeedItemsAsync(db, cancellationToken);

        _logger.LogInformation("Development data seeding completed.");
    }

    private static async Task SeedWarehouseAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Warehouses.AnyAsync(ct))
        {
            return;
        }

        Randomizer.Seed = new Random(8675309);

        Faker<Warehouse> faker = new Faker<Warehouse>("en")
            .CustomInstantiator(f => new Warehouse(
                code: $"ZAG{f.IndexFaker + 1:D2}",
                name: $"{f.Address.City()} Distribution Center",
                timeZone: "Europe/Zagreb"
            ));

        List<Warehouse> warehouses = faker.Generate(1);
        db.Warehouses.AddRange(warehouses);

        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedLocationsAsync(AppDbContext db, CancellationToken ct)
    {
        Warehouse warehouse = await db.Warehouses.FirstAsync(ct);

        // Get existing codes to avoid duplicates
        HashSet<string> existingCodes = await db.Locations
            .AsNoTracking()
            .Where(l => l.WarehouseId == warehouse.Id)
            .Select(l => l.Code)
            .ToHashSetAsync(ct);

        if (existingCodes.Count >= 5)
        {
            return;
        }

        Randomizer.Seed = new Random(8675309);
        var faker = new Faker("en");

        var locations = new List<Location>();
        for (int i = 0; i < 5; i++)
        {
            string type = faker.PickRandom(LocationTypes.All);
            string code;
            do
            {
                code = type switch
                {
                    LocationTypes.Stage => $"STAGE-{faker.Random.Int(1, 999):D3}",
                    LocationTypes.Rack => $"RACK-{faker.Random.Int(1, 999):D3}",
                    LocationTypes.Bin => $"BIN-{faker.Random.Int(1, 999):D3}",
                    LocationTypes.Floor => $"FLOOR-{faker.Random.Int(1, 999):D3}",
                    _ => $"LOC-{faker.Random.Int(1, 999):D3}"
                };
            }
            while (existingCodes.Contains(code));

            existingCodes.Add(code);

            int maxQty = type switch
            {
                LocationTypes.Rack => faker.Random.Int(500, 2000),
                LocationTypes.Bin => faker.Random.Int(100, 500),
                LocationTypes.Stage => faker.Random.Int(300, 800),
                _ => faker.Random.Int(200, 1000)
            };

            locations.Add(new Location(
                warehouseId: warehouse.Id,
                code: code,
                type: type,
                maxQty: maxQty
            ));
        }

        db.Locations.AddRange(locations);
        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedUnitOfMeasuresAsync(AppDbContext db, CancellationToken ct)
    {
        HashSet<string> existing = await db.UnitOfMeasures.AsNoTracking().Select(u => u.Symbol).ToHashSetAsync(StringComparer.OrdinalIgnoreCase, ct);

        (string, string)[] required =
        [
            ("pcs", "Piece"),
            ("kg", "Kilogram"),
            ("ltr", "Liter"),
            ("box", "Box")
        ];

        var newUnits = required
            .Where(u => !existing.Contains(u.Item1))
            .Select(u => new UnitOfMeasure(u.Item1, u.Item2))
            .ToList();

        if (newUnits.Count == 0)
        {
            return;
        }

        db.UnitOfMeasures.AddRange(newUnits);
        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedItemsAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Items.AnyAsync(ct))
        {
            return;
        }

        var uoms = await db.UnitOfMeasures
            .AsNoTracking()
            .Select(u => new { u.Id, u.Symbol })
            .ToListAsync(ct)
            ;

        if (uoms.Count == 0)
        {
            throw new InvalidOperationException("Cannot seed Items — UnitOfMeasure table is empty.");
        }

        Randomizer.Seed = new Random(8675309);

        Faker<Item> faker = new Faker<Item>("en")
            .CustomInstantiator(f =>
            {
                var randomUom = f.PickRandom(uoms);
                string sku = $"SKU-{f.IndexFaker + 1:D3}";
                string name = f.Commerce.ProductName();
                decimal cost = Math.Round(f.Random.Decimal(5, 50), 2);
                int leadTime = f.Random.Int(3, 14);
                int reorder = f.Random.Int(10, 100);

                return new Item(
                    sku: sku,
                    name: name,
                    unitOfMeasureId: randomUom.Id,
                    standardCost: cost,
                    leadTimeDays: leadTime,
                    reorderPoint: reorder
                );
            });

        List<Item> items = faker.Generate(10);
        db.Items.AddRange(items);
        await db.SaveChangesAsync(ct);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
