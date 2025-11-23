using MagnaWms.Domain.PutawayAggregate;
using MagnaWms.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class PutawayTaskConfiguration : AggregateRootConfigurationBase<PutawayTask>
{
    public override void Configure(EntityTypeBuilder<PutawayTask> builder)
    {
        base.Configure(builder);

        builder.ToTable("PutawayTask", "wms");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.WarehouseId).IsRequired();
        builder.Property(p => p.ReceiptId).IsRequired();
        builder.Property(p => p.ReceiptLineId).IsRequired();
        builder.Property(p => p.ItemId).IsRequired();

        builder.Property(p => p.QuantityToPutaway)
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        builder.Property(p => p.QuantityCompleted)
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.CreatedByUserId).IsRequired();
        builder.Property(p => p.CompletedByUserId).IsRequired(false);
        builder.Property(p => p.CompletedUtc).HasColumnType("datetime2");
    }
}
