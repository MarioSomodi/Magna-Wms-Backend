using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authorization;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Roles.Repository;
using MagnaWms.Contracts.Authorization;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.Authorization;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Roles.Commands.UpdateRolePermissions;

public sealed class UpdateRolePermissionsCommandHandler
    : IRequestHandler<UpdateRolePermissionsCommand, Result<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateRolePermissionsCommandHandler(
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
        UpdateRolePermissionsCommand request,
        CancellationToken cancellationToken)
    {
        Role? role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return Result<RoleDto>.Failure(
                new Error(ErrorCode.NotFound, $"Role with ID {request.RoleId} was not found."));
        }

        if (role.Name.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
        {
            return Result<RoleDto>.Failure(
                new Error(ErrorCode.Forbidden, $"Not high enough permissions to change this role."));
        }

        IReadOnlyList<Permission> allPerms = await _permissionRepository.GetByKeysAsync(request.PermissionKeys, cancellationToken);

        IReadOnlyList<RolePermission> existing = await _roleRepository.GetRolePermissions(role.Id, cancellationToken);
        _roleRepository.RemoveRolePermissionsRange(existing);

        if (allPerms.Count > 0)
        {
            IEnumerable<RolePermission> newLinks = allPerms.Select(p => new RolePermission(role.Id, p.Id));
            await _roleRepository.AddRolePermissionsRangeAsync(newLinks, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        IReadOnlyList<string> permKeys = allPerms.Select(p => p.Key).ToList();
        RoleDto dto = _mapper.Map<RoleDto>((role, permKeys));

        return Result<RoleDto>.Success(dto);
    }
}
