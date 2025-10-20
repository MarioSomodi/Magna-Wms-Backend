using MagnaWms.Domain.UnitOfMeasureAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.ToTable("UnitOfMeasure");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Symbol)
               .IsRequired()
               .HasMaxLength(16);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(64);

        builder.Property(x => x.CreatedUtc)
               .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(x => x.UpdatedUtc)
               .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(x => x.RowVersion)
               .IsRowVersion();

        builder.HasIndex(x => x.Symbol)
               .IsUnique();
    }
}
