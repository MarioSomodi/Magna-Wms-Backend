using MagnaWms.Domain.ReceiptAggregate;
using MagnaWms.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class ReceiptConfiguration : AggregateRootConfigurationBase<Receipt>
{
    public override void Configure(EntityTypeBuilder<Receipt> builder)
    {
        base.Configure(builder);

        builder.ToTable("Receipt", "wms");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.WarehouseId)
            .IsRequired();

        builder.Property(r => r.ReceiptNumber)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(r => r.ExternalReference)
            .HasMaxLength(128);

        builder.Property(r => r.ExpectedArrivalDate)
            .HasColumnType("datetime2");

        builder.Property(r => r.CreatedByUserId)
            .IsRequired();

        builder.Property(r => r.ReceivedByUserId)
            .IsRequired(false);

        builder.Property(r => r.ClosedUtc)
            .HasColumnType("datetime2");

        builder.Property(r => r.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasMany(r => r.Lines)
            .WithOne()
            .HasForeignKey(l => l.ReceiptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
