using MagnaWms.Domain.ItemAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;
public sealed class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Item");

        builder.HasKey(x => x.ItemID);

        builder.Property(x => x.Sku)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(x => x.Sku)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.BaseUom)
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(x => x.BaseUomFull)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.StandardCost)
            .HasColumnType("decimal(18,4)");

        builder.Property(x => x.LeadTimeDays);
        builder.Property(x => x.ReorderPoint);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(x => x.UpdatedUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()");
    }
}
