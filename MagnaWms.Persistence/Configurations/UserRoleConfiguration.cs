using MagnaWms.Domain.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations;
public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRole");

        builder.HasKey(ur => ur.Id);

        builder.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();
    }
}
