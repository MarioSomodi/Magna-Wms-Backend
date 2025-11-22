using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Receipts.Commands.ReceiveReceiptLine;

public sealed record ReceiveReceiptLineCommand(
    long ReceiptId,
    long LineId,
    decimal Quantity,
    long ToLocationId,
    string? Notes
) : IRequest<Result<ReceiptDto>>;
