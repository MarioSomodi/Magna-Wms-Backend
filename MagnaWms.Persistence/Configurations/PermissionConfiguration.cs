using MagnaWms.Domain.Authorization;
using MagnaWms.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;
public sealed class PermissionConfiguration : AggregateRootConfigurationBase<Permission>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);

        builder.ToTable("Permission");

        builder.Property(p => p.Key)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(p => p.Key).IsUnique();
    }
}
