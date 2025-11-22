using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Receipts.Queries.GetReceiptLines;

public sealed record GetReceiptLinesQuery(long ReceiptId)
    : IRequest<Result<IReadOnlyList<ReceiptLineDto>>>;
