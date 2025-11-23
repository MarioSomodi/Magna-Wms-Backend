using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Pick;
using MediatR;

namespace MagnaWms.Application.PickTasks.Commands.StartPickTask;

public sealed record StartPickTaskCommand(long TaskId)
    : IRequest<Result<PickTaskDto>>;
