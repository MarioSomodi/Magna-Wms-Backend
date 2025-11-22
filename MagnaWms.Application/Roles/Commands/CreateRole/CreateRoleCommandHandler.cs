using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Roles.Repository;
using MagnaWms.Application.Core.Abstractions.Authorization;
using MagnaWms.Contracts.Authorization;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.Authorization;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandHandler
    : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateRoleCommandHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<RoleDto>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        Role? existing = await _roleRepository.GetByNameAsync(request.Name, cancellationToken);

        if (existing is not null)
        {
            return Result<RoleDto>.Failure(
                new Error(ErrorCode.Conflict, $"Role '{request.Name}' already exists."));
        }

        var role = new Role(request.Name, request.Description);
        await _roleRepository.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        IReadOnlyList<Permission> perms =
            await _permissionRepository.GetByKeysAsync(request.PermissionKeys, cancellationToken);

        if (perms.Count > 0)
        {
            IEnumerable<RolePermission> links = perms.Select(p => new RolePermission(role.Id, p.Id));
            await _roleRepository.AddRolePermissionsRangeAsync(links, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        IReadOnlyList<string> permKeys = perms.Select(p => p.Key).ToList();

        RoleDto dto = _mapper.Map<RoleDto>((role, permKeys));

        return Result<RoleDto>.Success(dto);
    }
}
