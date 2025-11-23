using MagnaWms.Domain.PickTaskAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class PickTaskLineConfiguration : IEntityTypeConfiguration<PickTaskLine>
{
    public void Configure(EntityTypeBuilder<PickTaskLine> builder)
    {
        builder.ToTable("PickTaskLine", "wms");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.PickTaskId).IsRequired();
        builder.Property(l => l.ItemId).IsRequired();
        builder.Property(l => l.LocationId).IsRequired();

        builder.Property(l => l.QuantityToPick)
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        builder.Property(l => l.QuantityPicked)
            .HasColumnType("decimal(18,4)")
            .IsRequired();
    }
}
