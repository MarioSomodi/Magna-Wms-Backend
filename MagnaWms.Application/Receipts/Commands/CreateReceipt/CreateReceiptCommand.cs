using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Receipts.Commands.CreateReceipt;

public sealed record CreateReceiptCommand(
    long WarehouseId,
    string ReceiptNumber,
    string? ExternalReference,
    DateTime? ExpectedArrivalDate,
    IReadOnlyList<CreateReceiptLineRequest> Lines
) : IRequest<Result<ReceiptDto>>;
