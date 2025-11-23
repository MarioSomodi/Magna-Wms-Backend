using MagnaWms.Domain.Core.Exceptions;
using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.PickTaskAggregate;

public sealed class PickTask : AggregateRoot
{
    private readonly List<PickTaskLine> _lines = [];

    private PickTask() { }

    public PickTask(
        long warehouseId,
        long salesOrderId,
        long createdByUserId)
    {
        WarehouseId = warehouseId;
        SalesOrderId = salesOrderId;
        CreatedByUserId = createdByUserId;

        Status = PickTaskStatus.Open;
    }

    public long WarehouseId { get; private set; }
    public long SalesOrderId { get; private set; }

    public long CreatedByUserId { get; private set; }
    public long? CompletedByUserId { get; private set; }
    public DateTime? CompletedUtc { get; private set; }

    public PickTaskStatus Status { get; private set; }

    public IReadOnlyCollection<PickTaskLine> Lines => _lines.AsReadOnly();

    public void AddLine(long itemId, long locationId, decimal quantityToPick)
    {
        if (Status != PickTaskStatus.Open)
        {
            throw new DomainException("Cannot add lines once picking has started or completed.");
        }

        if (quantityToPick <= 0)
        {
            throw new DomainException("Quantity to pick must be positive.");
        }

        _lines.Add(new PickTaskLine(Id, itemId, locationId, quantityToPick));
    }

    public void Start()
    {
        if (Status != PickTaskStatus.Open)
        {
            throw new DomainException($"Cannot start pick task in status '{Status}'.");
        }

        Status = PickTaskStatus.InProgress;
    }

    public void RegisterPick(long lineId, decimal qty, long userId)
    {
        if (Status is not PickTaskStatus.Open and not PickTaskStatus.InProgress)
        {
            throw new DomainException($"Cannot pick lines in status '{Status}'.");
        }

        PickTaskLine line = GetLine(lineId);
        line.Pick(qty);

        Status = PickTaskStatus.InProgress;

        if (_lines.All(l => l.IsCompleted))
        {
            Status = PickTaskStatus.Completed;
            CompletedByUserId = userId;
            CompletedUtc = DateTime.UtcNow;
        }
    }

    private PickTaskLine GetLine(long lineId) =>
        _lines.FirstOrDefault(l => l.Id == lineId)
        ?? throw new DomainException($"Pick task line {lineId} not found.");
}
