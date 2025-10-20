using MagnaWms.Domain.Core.Exceptions;
using MagnaWms.Domain.Core.Primitives;
using MagnaWms.Domain.WarehouseAggregate;

namespace MagnaWms.Domain.LocationAggregate;

/// <summary>
/// Aggregate root represents a specific, addressable place inside a warehouse.
/// Examples: "RACK-A1", "BIN-001", "STAGE-01".
/// Used for putaway, picking, and physical stock tracking.
/// </summary>
public sealed class Location : AggregateRoot
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
    }

    public long WarehouseId { get; private set; }

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

    public Warehouse Warehouse { get; private set; } = default!;

    #region Business rules
    public void ChangeCapacity(int? maxQty) => SetCapacity(maxQty);

    public void Deactivate()
    {
        if (!IsActive)
        {
            throw new DomainException($"Location '{Code}' is already inactive.");
        }

        IsActive = false;
    }

    public void Reactivate()
    {
        if (IsActive)
        {
            throw new DomainException($"Location '{Code}' is already active.");
        }

        IsActive = true;
    }
    #endregion

    #region Private setters
    private void SetWarehouse(long id)
    {
        if (id <= 0)
        {
            throw new DomainException("Location must belong to a valid warehouse.");
        }

        WarehouseId = id;
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

        if (!LocationTypes.All.Contains(type, StringComparer.OrdinalIgnoreCase))
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
    #endregion
}
