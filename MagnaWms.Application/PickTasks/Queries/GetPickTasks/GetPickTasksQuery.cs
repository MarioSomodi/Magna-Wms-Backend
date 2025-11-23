using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Pick;
using MediatR;

namespace MagnaWms.Application.PickTasks.Queries.GetPickTasks;

public sealed record GetPickTasksQuery(long WarehouseId)
    : IRequest<Result<IReadOnlyList<PickTaskDto>>>;
