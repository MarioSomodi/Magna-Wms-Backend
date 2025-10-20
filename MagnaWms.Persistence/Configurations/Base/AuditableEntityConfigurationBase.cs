using MagnaWms.Domain.Core.Primitives;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Configurations.Base;

public abstract class AuditableEntityConfigurationBase<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IAuditableEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(e => e.CreatedUtc)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(e => e.UpdatedUtc)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
    }
}
