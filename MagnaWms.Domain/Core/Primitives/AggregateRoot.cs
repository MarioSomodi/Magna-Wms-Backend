namespace MagnaWms.Domain.Core.Primitives;

public abstract class AggregateRoot : Entity, IAuditableEntity
{
    /// <summary>
    /// Concurrency token (ROWVERSION). 
    /// Updated automatically on every modification to prevent lost updates.
    /// </summary>
    public byte[] RowVersion { get; protected set; } = Array.Empty<byte>();

    /// <summary>
    /// UTC timestamp when the record was created.
    /// </summary>
    public DateTime CreatedUtc { get; protected set; }
    /// <summary>
    /// UTC timestamp when the record was last updated.
    /// </summary>
    public DateTime UpdatedUtc { get; protected set; }

    DateTime IAuditableEntity.CreatedUtc
    {
        get => CreatedUtc;
        set => CreatedUtc = value;
    }

    DateTime IAuditableEntity.UpdatedUtc
    {
        get => UpdatedUtc;
        set => UpdatedUtc = value;
    }
}
