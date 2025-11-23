using MagnaWms.Domain.PickTaskAggregate;
using MagnaWms.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class PickTaskConfiguration : AggregateRootConfigurationBase<PickTask>
{
    public override void Configure(EntityTypeBuilder<PickTask> builder)
    {
        base.Configure(builder);

        builder.ToTable("PickTask", "wms");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.WarehouseId).IsRequired();
        builder.Property(p => p.SalesOrderId).IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.CreatedByUserId).IsRequired();
        builder.Property(p => p.CompletedByUserId).IsRequired(false);
        builder.Property(p => p.CompletedUtc)
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.HasMany<PickTaskLine>()
            .WithOne()
            .HasForeignKey(l => l.PickTaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
