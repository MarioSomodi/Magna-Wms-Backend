using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Roles.Repository;
using MagnaWms.Contracts.Authorization;
using MagnaWms.Domain.Authorization;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Roles.Queries.GetAllRoles;

public sealed class GetAllRolesQueryHandler
    : IRequestHandler<GetAllRolesQuery, Result<IReadOnlyList<RoleDto>>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public GetAllRolesQueryHandler(IRoleRepository roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<RoleDto>>> Handle(
        GetAllRolesQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Role> roles = await _roleRepository.GetAllAsync(cancellationToken);

        var list = new List<RoleDto>(roles.Count);

        foreach (Role role in roles)
        {
            IReadOnlyList<string> perms = await _roleRepository.GetPermissionKeysAsync(role.Id, cancellationToken);
            RoleDto dto = _mapper.Map<RoleDto>((role, perms));
            list.Add(dto);
        }

        return Result<IReadOnlyList<RoleDto>>.Success(list);
    }
}
