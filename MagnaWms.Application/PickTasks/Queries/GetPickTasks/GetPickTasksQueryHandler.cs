using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.PickTasks.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Pick;
using MagnaWms.Domain.PickTaskAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.PickTasks.Queries.GetPickTasks;

public sealed class GetPickTasksQueryHandler
    : IRequestHandler<GetPickTasksQuery, Result<IReadOnlyList<PickTaskDto>>>
{
    private readonly IPickTaskRepository _tasks;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetPickTasksQueryHandler(
        IPickTaskRepository tasks,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _tasks = tasks;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<PickTaskDto>>> Handle(
        GetPickTasksQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(request.WarehouseId))
        {
            return Result<IReadOnlyList<PickTaskDto>>.Failure(
                new Error(ErrorCode.Forbidden, "Not allowed."));
        }

        IReadOnlyList<PickTask> tasks =
            await _tasks.GetOpenByWarehouseAsync(request.WarehouseId, cancellationToken);

        IReadOnlyList<PickTaskDto> dtos = tasks
            .Select(t => _mapper.Map<PickTaskDto>(t))
            .ToList();

        return Result<IReadOnlyList<PickTaskDto>>.Success(dtos);
    }
}
