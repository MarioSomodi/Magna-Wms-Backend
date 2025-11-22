using MagnaWms.Application.Core.Abstractions.Authorization;
using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Authorization;
using MagnaWms.Domain.Authorization;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Permissions.Queries.GetAllPermissions;

public sealed class GetAllPermissionsQueryHandler
    : IRequestHandler<GetAllPermissionsQuery, Result<IReadOnlyList<PermissionDto>>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMapper _mapper;

    public GetAllPermissionsQueryHandler(IPermissionRepository permissionRepository, IMapper mapper)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<PermissionDto>>> Handle(
        GetAllPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Permission> permissions = await _permissionRepository.GetAllAsync(cancellationToken);

        var dtoList = permissions
            .Select(p => _mapper.Map<PermissionDto>(p))
            .ToList();

        return Result<IReadOnlyList<PermissionDto>>.Success(dtoList);
    }
}
