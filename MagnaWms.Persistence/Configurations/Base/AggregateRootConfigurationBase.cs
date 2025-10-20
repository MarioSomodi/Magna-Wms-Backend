using MagnaWms.Domain.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnaWms.Persistence.Configurations.Base;

/// <summary>
/// Base EF configuration for all aggregate roots,
/// handling common audit fields and concurrency tokens.
/// </summary>
/// <typeparam name="TAggregateRoot">Aggregate root type.</typeparam>
public abstract class AggregateRootConfigurationBase<TAggregateRoot> : IEntityTypeConfiguration<TAggregateRoot>
    where TAggregateRoot : AggregateRoot
{
    public virtual void Configure(EntityTypeBuilder<TAggregateRoot> builder)
    {
        // Concurrency token (ROWVERSION)
        builder.Property(e => e.RowVersion)
               .IsRowVersion()
               .IsConcurrencyToken();

        // CreatedUtc
        builder.Property(e => e.CreatedUtc)
               .IsRequired()
               .HasDefaultValueSql("SYSUTCDATETIME()");

        // UpdatedUtc
        builder.Property(e => e.UpdatedUtc)
               .IsRequired()
               .HasDefaultValueSql("SYSUTCDATETIME()");
    }
}
