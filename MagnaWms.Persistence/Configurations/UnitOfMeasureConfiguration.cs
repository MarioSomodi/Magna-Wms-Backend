using MagnaWms.Domain.UnitOfMeasureAggregate;
using MagnaWms.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class UnitOfMeasureConfiguration : AggregateRootConfigurationBase<UnitOfMeasure>
{
    public override void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        base.Configure(builder);

        builder.ToTable("UnitOfMeasure");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Symbol)
               .IsRequired()
               .HasMaxLength(16);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(64);

        builder.HasIndex(x => x.Symbol)
               .IsUnique();
    }
}
