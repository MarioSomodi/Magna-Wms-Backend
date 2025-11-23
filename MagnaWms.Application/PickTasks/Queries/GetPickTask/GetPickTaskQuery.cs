using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Pick;
using MediatR;

namespace MagnaWms.Application.PickTasks.Queries.GetPickTask;

public sealed record GetPickTaskQuery(long TaskId)
    : IRequest<Result<PickTaskDto>>;
