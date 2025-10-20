using MagnaWms.Domain.Core.Exceptions;
using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.WarehouseAggregate;

/// <summary>
/// Represents a physical warehouse or distribution center.
/// The Aggregate root for storage, inventory, and operational activities.
/// </summary>
public sealed class Warehouse : AggregateRoot
{
    // Needed for EF core init
    private Warehouse() { }

    public Warehouse(string code, string name, string timeZone)
    {
        SetCode(code);
        SetName(name);
        SetTimeZone(timeZone);

        IsActive = true;
    }
    /// <summary>
    /// Unique warehouse code (e.g., "ZAG01"). 
    /// Short human-readable identifier used across systems.
    /// </summary>
    public string Code { get; private set; } = default!;

    /// <summary>
    /// Descriptive warehouse name (e.g., "Zagreb Central Distribution Center").
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// IANA timezone string (e.g., "Europe/Zagreb") used for localized times.
    /// </summary>
    public string TimeZone { get; private set; } = default!;

    /// <summary>
    /// Indicates if this warehouse is operational.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    #region Business rules
    public void Rename(string newName) => SetName(newName);

    public void ChangeTimeZone(string newTimeZone) => SetTimeZone(newTimeZone);

    public void Deactivate()
    {
        if (!IsActive)
        {
            throw new DomainException($"Warehouse '{Code}' is already inactive.");
        }

        IsActive = false;
    }

    public void Reactivate()
    {
        if (IsActive)
        {
            throw new DomainException($"Warehouse '{Code}' is already active.");
        }

        IsActive = true;
    }
    #endregion

    #region Private setters
    private void SetCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Warehouse code cannot be empty.");
        }

        Code = code.Trim().ToUpperInvariant();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Warehouse name cannot be empty.");
        }

        Name = name.Trim();
    }

    private void SetTimeZone(string tz)
    {
        if (string.IsNullOrWhiteSpace(tz))
        {
            throw new DomainException("Time zone cannot be empty.");
        }

        TimeZone = tz.Trim();
    }
    #endregion
}
