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

        await db.Database.MigrateAsync(cancellationToken).ConfigureAwait(false); // ensure DB up to date

        await SeedWarehouseAsync(db, cancellationToken).ConfigureAwait(false);
        await SeedLocationsAsync(db, cancellationToken).ConfigureAwait(false);
        await SeedUnitOfMeasuresAsync(db, cancellationToken).ConfigureAwait(false);
        await SeedItemsAsync(db, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Development data seeding completed.");
    }

    private static async Task SeedWarehouseAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Warehouses.AnyAsync(ct).ConfigureAwait(false))
        {
            return;
        }

        var warehouse = new Warehouse(code: "ZAG01", name: "Zagreb Central Distribution Center", timeZone: "Europe/Zagreb");

        db.Warehouses.Add(warehouse);

        await db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private static async Task SeedLocationsAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Locations.AnyAsync(ct).ConfigureAwait(false))
        {
            return;
        }

        Warehouse warehouse = await db.Warehouses.FirstAsync(ct).ConfigureAwait(false);

    private static async Task SeedUnitOfMeasuresAsync(AppDbContext db, CancellationToken ct)
    {
        HashSet<string> existing = await db.UnitOfMeasures.AsNoTracking().Select(u => u.Symbol).ToHashSetAsync(StringComparer.OrdinalIgnoreCase, ct).ConfigureAwait(false);

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
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private static async Task SeedItemsAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Items.AnyAsync(ct).ConfigureAwait(false))
        {
            return;
        }

        var uoms = await db.UnitOfMeasures
            .AsNoTracking()
            .Select(u => new { u.Id, u.Symbol })
            .ToListAsync(ct)
            .ConfigureAwait(false);

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
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
