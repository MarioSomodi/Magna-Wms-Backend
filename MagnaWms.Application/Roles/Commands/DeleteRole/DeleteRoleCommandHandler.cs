using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Roles.Repository;
using MagnaWms.Application.Users.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Authorization;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.Authorization;
using MediatR;

namespace MagnaWms.Application.Roles.Commands.DeleteRole;

public sealed class DeleteRoleCommandHandler
    : IRequestHandler<DeleteRoleCommand, Result<Success>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoleCommandHandler(
        IRoleRepository roleRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        Role? role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return Result<Success>.Failure(
                new Error(ErrorCode.NotFound, $"Role with ID {request.RoleId} was not found."));
        }

        bool hasUsers = await _userRepository.IsRoleAssignedToAnyUser(request.RoleId, cancellationToken);

        if (hasUsers)
        {
            return Result<Success>.Failure(
                new Error(ErrorCode.Conflict, "Role cannot be deleted while assigned to users."));
        }

        if (role.Name.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
        {
            return Result<Success>.Failure(
                new Error(ErrorCode.Forbidden, $"Not high enough permissions to delete this role."));
        }

        IReadOnlyList<RolePermission> existingRolePermissions = await _roleRepository.GetRolePermissions(request.RoleId, cancellationToken);

        if (existingRolePermissions.Count > 0)
        {
            _roleRepository.RemoveRolePermissionsRange(existingRolePermissions);
        }

        _roleRepository.Remove(role);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Success>.Success(new Success("Deleted"));
    }
}
