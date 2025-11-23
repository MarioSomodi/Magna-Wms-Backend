using MagnaWms.Domain.ShipmentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class ShipmentLineConfiguration : IEntityTypeConfiguration<ShipmentLine>
{
    public void Configure(EntityTypeBuilder<ShipmentLine> builder)
    {
        builder.ToTable("ShipmentLine", "wms");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.ShipmentId).IsRequired();
        builder.Property(l => l.ItemId).IsRequired();

        builder.Property(l => l.QuantityShipped)
            .HasColumnType("decimal(18,4)")
            .IsRequired();
    }
}
