using MagnaWms.Domain.Core.Exceptions;
using MagnaWms.Domain.WarehouseAggregate;

namespace MagnaWms.Domain.LocationAggregate;

/// <summary>
/// Aggregate root represents a specific, addressable place inside a warehouse.
/// Examples: "RACK-A1", "BIN-001", "STAGE-01".
/// Used for putaway, picking, and physical stock tracking.
/// </summary>
public sealed class Location
{
    // Needed for EF core init
    private Location() { }

    public Location(long warehouseId, string code, string type, int? maxQty = null)
    {
        SetWarehouse(warehouseId);
        SetCode(code);
        SetType(type);
        SetCapacity(maxQty);
        IsActive = true;
        CreatedUtc = DateTime.UtcNow;
        UpdatedUtc = DateTime.UtcNow;
    }

    public long LocationID { get; private set; }

    public long WarehouseID { get; private set; }

    /// <summary>
    /// Unique code within a warehouse identifying the storage address.
    /// </summary>
    public string Code { get; private set; } = default!;

    /// <summary>
    /// Type classification: "RACK", "BIN", "FLOOR", or "STAGE".
    /// Defines physical behavior and capacity rules.
    /// </summary>
    public string Type { get; private set; } = LocationTypes.Bin;

    /// <summary>
    /// Maximum allowed quantity (optional, null = unrestricted).
    /// Used for capacity management.
    /// </summary>
    public int? MaxQty { get; private set; }

    /// <summary>
    /// Indicates if this location is active and used in operations.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// UTC timestamp when the record was created.
    /// </summary>
    public DateTime CreatedUtc { get; private set; }

    /// <summary>
    /// UTC timestamp when the record was last updated.
    /// </summary>
    public DateTime UpdatedUtc { get; private set; }

    public Warehouse Warehouse { get; private set; } = default!;

    #region Business rules
    public void ChangeCapacity(int? maxQty)
    {
        SetCapacity(maxQty);
        Touch();
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            throw new DomainException($"Location '{Code}' is already inactive.");
        }

        IsActive = false;
        Touch();
    }

    public void Reactivate()
    {
        if (IsActive)
        {
            throw new DomainException($"Location '{Code}' is already active.");
        }

        IsActive = true;
        Touch();
    }
    #endregion

    #region Private setters
    private void SetWarehouse(long id)
    {
        if (id <= 0)
        {
            throw new DomainException("Location must belong to a valid warehouse.");
        }

        WarehouseID = id;
    }

    private void SetCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Location code cannot be empty.");
        }

        Code = code.Trim().ToUpperInvariant();
    }

    private void SetType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new DomainException("Location type cannot be empty.");
        }

        if (!LocationTypes.All.Contains(type.ToUpperInvariant()))
        {
            throw new DomainException($"Invalid location type '{type}'.");
        }

        Type = type.ToUpperInvariant();
    }

    private void SetCapacity(int? maxQty)
    {
        if (maxQty is < 0)
        {
            throw new DomainException("MaxQty cannot be negative.");
        }

        MaxQty = maxQty;
    }

    private void Touch() => UpdatedUtc = DateTime.UtcNow;
    #endregion
}
