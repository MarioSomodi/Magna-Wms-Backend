using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.PickTasks.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Pick;
using MagnaWms.Domain.PickTaskAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.PickTasks.Queries.GetPickTask;

public sealed class GetPickTaskQueryHandler
    : IRequestHandler<GetPickTaskQuery, Result<PickTaskDto>>
{
    private readonly IPickTaskRepository _tasks;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetPickTaskQueryHandler(
        IPickTaskRepository tasks,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _tasks = tasks;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<PickTaskDto>> Handle(
        GetPickTaskQuery request,
        CancellationToken cancellationToken)
    {
        PickTask? task = await _tasks.GetWithLinesAsync(request.TaskId, cancellationToken);

        if (task is null)
        {
            return Result<PickTaskDto>.Failure(
                new Error(ErrorCode.NotFound, "Pick task not found."));
        }

        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(task.WarehouseId))
        {
            return Result<PickTaskDto>.Failure(
                new Error(ErrorCode.Forbidden, "Not allowed."));
        }

        return Result<PickTaskDto>.Success(_mapper.Map<PickTaskDto>(task));
    }
}
