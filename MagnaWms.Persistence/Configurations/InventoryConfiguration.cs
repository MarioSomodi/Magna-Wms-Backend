using MagnaWms.Domain.InventoryAggregate;
using MagnaWms.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class InventoryConfiguration : AggregateRootConfigurationBase<Inventory>
{
    public override void Configure(EntityTypeBuilder<Inventory> builder)
    {
        base.Configure(builder);

        builder.ToTable("Inventory");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.WarehouseId)
            .IsRequired();

        builder.Property(i => i.LocationId)
            .IsRequired();

        builder.Property(i => i.ItemId)
            .IsRequired();

        builder.Property(i => i.QuantityOnHand)
            .IsRequired()
            .HasColumnType("decimal(18, 4)");

        builder.Property(i => i.QuantityAllocated)
            .IsRequired()
            .HasColumnType("decimal(18, 4)");

        builder.Ignore(i => i.QuantityAvailable);

        builder.HasIndex(i => new { i.WarehouseId, i.LocationId, i.ItemId })
            .IsUnique();
    }
}
