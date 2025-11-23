using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Putaway;
using MediatR;

namespace MagnaWms.Application.Putaway.Commands.CreatePutaway;

public sealed record CreatePutawayTaskCommand(
    long ReceiptId,
    long ReceiptLineId,
    decimal QuantityToPutaway
) : IRequest<Result<PutawayTaskDto>>;
