using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Putaway.Commands.CreatePutawayTask;

public sealed record CreatePutawayTaskCommand(
    long ReceiptId,
    long ReceiptLineId,
    decimal QuantityToPutaway
) : IRequest<Result<PutawayTaskDto>>;
