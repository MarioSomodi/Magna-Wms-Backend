using MagnaWms.Domain.Core.Exceptions;
using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.ReceiptAggregate;

public sealed class Receipt : AggregateRoot
{
    private readonly List<ReceiptLine> _lines = new();

    private Receipt() { }

    public Receipt(
        long warehouseId,
        string receiptNumber,
        string? externalReference,
        DateTime? expectedArrivalDate,
        long createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(receiptNumber))
        {
            throw new DomainException("Receipt number cannot be empty.");
        }

        WarehouseId = warehouseId;
        ReceiptNumber = receiptNumber;
        ExternalReference = externalReference;
        ExpectedArrivalDate = expectedArrivalDate;
        CreatedByUserId = createdByUserId;

        Status = ReceiptStatus.Open;
    }

    public long WarehouseId { get; private set; }
    public string ReceiptNumber { get; private set; } = null!;
    public string? ExternalReference { get; private set; }
    public DateTime? ExpectedArrivalDate { get; private set; }

    public long CreatedByUserId { get; private set; }
    public long? ReceivedByUserId { get; private set; }
    public DateTime? ClosedUtc { get; private set; }

    public ReceiptStatus Status { get; private set; }
    public IReadOnlyCollection<ReceiptLine> Lines => _lines.AsReadOnly();

    public void AddLine(long itemId, decimal expectedQty)
    {
        if (Status != ReceiptStatus.Open)
        {
            throw new DomainException("Cannot add lines to a closed receipt.");
        }

        _lines.Add(new ReceiptLine(Id, itemId, expectedQty));
    }

    public void Receive(long userId, long lineId, decimal quantity)
    {
        if (Status != ReceiptStatus.Open)
        {
            throw new DomainException("Receipt is closed.");
        }

        ReceiptLine line = _lines.FirstOrDefault(l => l.Id == lineId)
            ?? throw new DomainException($"Receipt line {lineId} not found.");

        line.Receive(quantity);

        if (_lines.TrueForAll(l => l.IsFullyReceived))
        {
            Status = ReceiptStatus.Closed;
            ReceivedByUserId = userId;
            ClosedUtc = DateTime.UtcNow;
        }
    }
}
