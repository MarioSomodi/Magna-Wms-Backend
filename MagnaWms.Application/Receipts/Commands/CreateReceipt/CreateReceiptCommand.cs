using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Receipts;
using MediatR;

namespace MagnaWms.Application.Receipts.Commands.CreateReceipt;

public sealed record CreateReceiptCommand(
    long WarehouseId,
    string ReceiptNumber,
    string? ExternalReference,
    DateTime? ExpectedArrivalDate,
    IReadOnlyList<CreateReceiptLineRequest> Lines
) : IRequest<Result<ReceiptDto>>;
