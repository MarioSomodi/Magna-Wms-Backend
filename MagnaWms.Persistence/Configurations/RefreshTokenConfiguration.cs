using MagnaWms.Domain.RefreshTokenAggregate;
using MagnaWms.Domain.UserAggregate;
using MagnaWms.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : AggregateRootConfigurationBase<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);
        builder.ToTable("RefreshToken");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.TokenHash).IsRequired();
        builder.Property(r => r.CreatedByIp).HasMaxLength(45);
        builder.Property(r => r.RevokedByIp).HasMaxLength(45);

        builder.HasIndex(r => r.TokenHash).IsUnique(false);

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
