using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Receipts;
using MediatR;

namespace MagnaWms.Application.Receipts.Commands.ReceiveReceiptLine;

public sealed record ReceiveReceiptLineCommand(
    long ReceiptId,
    long LineId,
    decimal Quantity,
    long ToLocationId,
    string? Notes
) : IRequest<Result<ReceiptDto>>;
