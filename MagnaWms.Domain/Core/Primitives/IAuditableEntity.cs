namespace MagnaWms.Domain.Core.Primitives;

/// <summary>
/// Marks entities that should have audit timestamps automatically maintained.
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedUtc { get; set; }
    DateTime UpdatedUtc { get; set; }
}
