// MagnaWms.Persistence/Configurations/ForecastSeriesPointConfiguration.cs
using MagnaWms.Domain.ForecastAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class ForecastSeriesPointConfiguration : IEntityTypeConfiguration<ForecastSeriesPoint>
{
    public void Configure(EntityTypeBuilder<ForecastSeriesPoint> builder)
    {
        builder.ToTable("ForecastSeriesPoint", "wms");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ForecastSeriesId)
            .IsRequired();

        builder.Property(p => p.ForecastDateUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(p => p.Quantity)
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        builder.HasIndex(p => new { p.ForecastSeriesId, p.ForecastDateUtc });
    }
}
