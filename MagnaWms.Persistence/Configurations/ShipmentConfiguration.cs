using MagnaWms.Domain.ShipmentAggregate;
using MagnaWms.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class ShipmentConfiguration : AggregateRootConfigurationBase<Shipment>
{
    public override void Configure(EntityTypeBuilder<Shipment> builder)
    {
        base.Configure(builder);

        builder.ToTable("Shipment", "wms");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.WarehouseId).IsRequired();
        builder.Property(s => s.SalesOrderId).IsRequired();

        builder.Property(s => s.ShipmentNumber)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(s => s.Carrier)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(s => s.TrackingNumber)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(s => s.ShippedByUserId).IsRequired();

        builder.Property(s => s.ShippedUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasMany<ShipmentLine>()
            .WithOne()
            .HasForeignKey(l => l.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
