using MagnaWms.Domain.LocationAggregate;
using MagnaWms.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class LocationConfiguration : AggregateRootConfigurationBase<Location>
{
    public override void Configure(EntityTypeBuilder<Location> builder)
    {
        base.Configure(builder);

        string allowedTypes = string.Join("','", LocationTypes.All);

        builder.ToTable("Location", tb => tb.HasCheckConstraint("CK_Location_Type", $"Type IN ('{allowedTypes}')"));

        builder.HasKey(x => x.Id);

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

        builder.HasOne(l => l.Warehouse)
            .WithMany()
            .HasForeignKey(l => l.WarehouseID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
