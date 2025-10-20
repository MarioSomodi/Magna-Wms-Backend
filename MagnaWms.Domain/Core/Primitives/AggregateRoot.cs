namespace MagnaWms.Domain.Core.Primitives;

public abstract class AggregateRoot : Entity
{
    /// <summary>
    /// Concurrency token (ROWVERSION). 
    /// Updated automatically on every modification to prevent lost updates.
    /// </summary>
    public byte[] RowVersion { get; protected set; } = Array.Empty<byte>();

    /// <summary>
    /// UTC timestamp when the record was created.
    /// </summary>
    public DateTime CreatedUtc { get; protected set; } = DateTime.UtcNow;
    /// <summary>
    /// UTC timestamp when the record was last updated.
    /// </summary>
    public DateTime UpdatedUtc { get; protected set; } = DateTime.UtcNow;

    public void Touch() => UpdatedUtc = DateTime.UtcNow;
}
