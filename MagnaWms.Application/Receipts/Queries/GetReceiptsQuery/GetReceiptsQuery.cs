using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Receipts.Queries.GetReceipts;

public sealed record GetReceiptsQuery(long WarehouseId)
    : IRequest<Result<IReadOnlyList<ReceiptDto>>>;
