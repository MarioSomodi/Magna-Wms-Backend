using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Pick;
using MediatR;

namespace MagnaWms.Application.PickTasks.Commands.CreatePickTask;

public sealed record CreatePickTaskCommand(long SalesOrderId)
    : IRequest<Result<PickTaskDto>>;
