using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Pick;
using MediatR;

namespace MagnaWms.Application.PickTasks.Commands.CompletePickTask;

public sealed record CompletePickTaskCommand(long TaskId)
    : IRequest<Result<PickTaskDto>>;
