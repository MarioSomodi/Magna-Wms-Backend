using MagnaWms.Domain.ReceiptAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class ReceiptLineConfiguration : IEntityTypeConfiguration<ReceiptLine>
{
    public void Configure(EntityTypeBuilder<ReceiptLine> builder)
    {
        builder.ToTable("ReceiptLine", "wms");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.ReceiptId)
            .IsRequired();

        builder.Property(l => l.ItemId)
            .IsRequired();

        builder.Property(l => l.ItemSku)
            .HasMaxLength(64);

        builder.Property(l => l.ItemName)
            .HasMaxLength(256);

        builder.Property(l => l.UnitOfMeasure)
            .HasMaxLength(32);

        builder.Property(l => l.ExpectedQuantity)
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        builder.Property(l => l.ReceivedQuantity)
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        builder.Property(l => l.ToLocationId)
            .IsRequired(false);

        builder.Property(l => l.Notes)
            .HasMaxLength(512)
            .IsRequired(false);
    }
}
