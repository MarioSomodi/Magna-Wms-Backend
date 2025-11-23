using MagnaWms.Domain.ForecastAggregate;
using MagnaWms.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class ForecastSeriesConfiguration : AggregateRootConfigurationBase<ForecastSeries>
{
    public override void Configure(EntityTypeBuilder<ForecastSeries> builder)
    {
        base.Configure(builder);

        builder.ToTable("ForecastSeries", "wms");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.WarehouseId)
            .IsRequired();

        builder.Property(f => f.ItemId)
            .IsRequired();

        builder.Property(f => f.HorizonDays)
            .IsRequired();

        builder.Property(f => f.ModelInfo)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasMany(f => f.Points)
            .WithOne()
            .HasForeignKey(p => p.ForecastSeriesId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(f => new { f.WarehouseId, f.ItemId, f.CreatedUtc });
    }
}
