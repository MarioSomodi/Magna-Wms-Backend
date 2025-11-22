using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Roles.Repository;
using MagnaWms.Contracts.Authorization;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.Authorization;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Roles.Queries.GetRoleById;

public sealed class GetRoleByIdQueryHandler
    : IRequestHandler<GetRoleByIdQuery, Result<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public GetRoleByIdQueryHandler(IRoleRepository roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<Result<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        Role? role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return Result<RoleDto>.Failure(
                new Error(ErrorCode.NotFound, $"Role with ID {request.RoleId} was not found."));
        }

        IReadOnlyList<string> perms = await _roleRepository.GetPermissionKeysAsync(request.RoleId, cancellationToken);

        RoleDto dto = _mapper.Map<RoleDto>((role, perms));

        return Result<RoleDto>.Success(dto);
    }
}
