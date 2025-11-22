using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Authorization;
using MediatR;

namespace MagnaWms.Application.Roles.Commands.UpdateRolePermissions;

public sealed record UpdateRolePermissionsCommand(
    long RoleId,
    IReadOnlyList<string> PermissionKeys
) : IRequest<Result<RoleDto>>;
