using MagnaWms.Domain.InventoryAggregate;
using MagnaWms.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class InventoryLedgerEntryConfiguration : AggregateRootConfigurationBase<InventoryLedgerEntry>
{
    public override void Configure(EntityTypeBuilder<InventoryLedgerEntry> builder)
    {
        base.Configure(builder);

        builder.ToTable("InventoryLedger");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.WarehouseId).IsRequired();
        builder.Property(e => e.LocationId).IsRequired();
        builder.Property(e => e.ItemId).IsRequired();

        builder.Property(e => e.TimestampUtc)
            .IsRequired();

        builder.Property(e => e.QuantityChange)
            .IsRequired()
            .HasColumnType("decimal(18, 4)");

        builder.Property(e => e.ResultingQuantityOnHand)
            .IsRequired()
            .HasColumnType("decimal(18, 4)");

        builder.Property(e => e.MovementType)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(e => e.ReferenceType)
            .HasMaxLength(64);

        builder.Property(e => e.ReferenceNumber)
            .HasMaxLength(128);

        builder.HasIndex(e => new { e.WarehouseId, e.ItemId, e.TimestampUtc });
        builder.HasIndex(e => new { e.WarehouseId, e.LocationId, e.ItemId, e.TimestampUtc });
    }
}
