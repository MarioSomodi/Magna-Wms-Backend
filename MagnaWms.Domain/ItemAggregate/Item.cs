using MagnaWms.Domain.Core.Exceptions;

namespace MagnaWms.Domain.ItemAggregate;

/// <summary>
/// Aggregate root representing a stock-keeping unit (SKU) or product managed by the WMS.
/// Used across inbound, outbound, and inventory processes.
/// </summary>
public sealed class Item
{
    // Needed for EF core init
    private Item() { }

    public Item(
        string sku,
        string name,
        string baseUom,
        string baseUomFull,
        decimal? standardCost = null,
        int? leadTimeDays = null,
        int? reorderPoint = null)
    {
        SetSku(sku);
        SetName(name);
        SetBaseUom(baseUom, baseUomFull);
        UpdateAttributes(standardCost, leadTimeDays, reorderPoint);

        IsActive = true;
        CreatedUtc = DateTime.UtcNow;
        UpdatedUtc = DateTime.UtcNow;
    }

    public long ItemID { get; private set; }

    /// <summary>
    /// Stock Keeping Unit — globally unique business identifier (e.g., "ITEM-00123").
    /// </summary>
    public string Sku { get; private set; } = default!;

    /// <summary>
    /// Human-readable product name or description.
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Base unit of measure abbreviation (e.g., "kg", "pcs", "box").
    /// Determines the smallest trackable inventory unit.
    /// </summary>
    public string BaseUom { get; private set; } = default!;

    /// <summary>
    /// Full name of the base unit of measure (e.g., "kilogram", "piece", "box").
    /// Provides clarity in UI and reporting.
    /// </summary>
    public string BaseUomFull { get; private set; } = default!;

    /// <summary>
    /// Standard or average cost per base unit.
    /// Used for valuation and replenishment cost estimation.
    /// </summary>
    public decimal? StandardCost { get; private set; }

    /// <summary>
    /// Expected lead time (in days) from supplier order to warehouse delivery.
    /// Used by forecasting and replenishment logic.
    /// </summary>
    public int? LeadTimeDays { get; private set; }

    /// <summary>
    /// Reorder point (quantity threshold).
    /// When on-hand stock drops below this, replenishment is triggered.
    /// </summary>
    public int? ReorderPoint { get; private set; }

    /// <summary>
    /// Indicates if this item is active and available for use.
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
    #region Business rules
    public void UpdateInfo(string name, decimal? cost, int? leadTimeDays, int? reorderPoint)
    {
        SetName(name);
        UpdateAttributes(cost, leadTimeDays, reorderPoint);
        Touch();
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            throw new DomainException($"Item '{Sku}' is already inactive.");
        }

        IsActive = false;
        Touch();
    }

    public void Reactivate()
    {
        if (IsActive)
        {
            throw new DomainException($"Item '{Sku}' is already active.");
        }

        IsActive = true;
        Touch();
    }
    #endregion
    #region Private setters
    private void SetSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new DomainException("SKU cannot be empty.");
        }

        Sku = sku.Trim().ToUpperInvariant();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Item name cannot be empty.");
        }

        Name = name.Trim();
    }

    private void SetBaseUom(string abbrev, string full)
    {
        if (string.IsNullOrWhiteSpace(abbrev))
        {
            throw new DomainException("BaseUom cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(full))
        {
            throw new DomainException("BaseUomFull cannot be empty.");
        }

        BaseUom = abbrev.Trim()
                        .ToLowerInvariant();
        BaseUomFull = full.Trim();
    }

    private void UpdateAttributes(decimal? cost, int? leadTimeDays, int? reorderPoint)
    {
        if (leadTimeDays is < 0)
        {
            throw new DomainException("LeadTimeDays cannot be negative.");
        }

        if (reorderPoint is < 0)
        {
            throw new DomainException("ReorderPoint cannot be negative.");
        }

        if (cost is < 0)
        {
            throw new DomainException("StandardCost cannot be negative.");
        }

        StandardCost = cost;
        LeadTimeDays = leadTimeDays;
        ReorderPoint = reorderPoint;
    }

    private void Touch() => UpdatedUtc = DateTime.UtcNow;
    #endregion
}
