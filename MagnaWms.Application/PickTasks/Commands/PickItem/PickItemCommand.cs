using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Pick;
using MediatR;

namespace MagnaWms.Application.PickTasks.Commands.PickItem;

public sealed record PickItemCommand(
    long TaskId,
    long LineId,
    decimal QuantityPicked
) : IRequest<Result<PickTaskDto>>;
