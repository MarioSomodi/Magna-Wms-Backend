using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Receipts.Queries.GetReceipt;

public sealed record GetReceiptQuery(long ReceiptId)
    : IRequest<Result<ReceiptDto>>;
