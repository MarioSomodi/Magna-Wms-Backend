using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Receipts;
using MediatR;

namespace MagnaWms.Application.Receipts.Queries.GetReceiptLines;

public sealed record GetReceiptLinesQuery(long ReceiptId)
    : IRequest<Result<IReadOnlyList<ReceiptLineDto>>>;
