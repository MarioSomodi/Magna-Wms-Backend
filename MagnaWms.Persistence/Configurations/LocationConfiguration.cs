using MagnaWms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        string allowedTypes = string.Join("','", LocationTypes.All);

        builder.ToTable("Location", tb => tb.HasCheckConstraint("CK_Location_Type", $"Type IN ('{allowedTypes}')"));

        builder.HasKey(x => x.LocationID);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(x => new { x.WarehouseID, x.Code })
            .IsUnique();

        builder.Property(x => x.Type)
            .HasMaxLength(16);

        builder.Property(x => x.MaxQty);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(x => x.UpdatedUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasOne(l => l.Warehouse)
            .WithMany()
            .HasForeignKey(l => l.WarehouseID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
