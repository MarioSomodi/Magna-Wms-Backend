using MagnaWms.Domain.SalesOrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class SalesOrderLineConfiguration : IEntityTypeConfiguration<SalesOrderLine>
{
    public void Configure(EntityTypeBuilder<SalesOrderLine> builder)
    {
        builder.ToTable("SalesOrderLine", "wms");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.SalesOrderId)
            .IsRequired();

        builder.Property(l => l.ItemId)
            .IsRequired();

        builder.Property(l => l.QuantityOrdered)
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        builder.Property(l => l.QuantityAllocated)
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        builder.Property(l => l.QuantityPicked)
            .HasColumnType("decimal(18,4)")
            .IsRequired();
    }
}
