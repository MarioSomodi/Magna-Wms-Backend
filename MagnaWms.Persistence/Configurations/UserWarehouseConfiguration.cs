using MagnaWms.Domain.Authorization;
using MagnaWms.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class UserWarehouseConfiguration : AggregateRootConfigurationBase<UserWarehouse>
{
    public override void Configure(EntityTypeBuilder<UserWarehouse> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserWarehouse");

        builder.HasKey(uw => uw.Id);

        builder.Property(uw => uw.UserId)
               .IsRequired();

        builder.Property(uw => uw.WarehouseId)
               .IsRequired();

        builder.HasIndex(uw => new { uw.UserId, uw.WarehouseId })
               .IsUnique();
    }
}
