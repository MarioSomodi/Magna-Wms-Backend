using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Authorization;
using MediatR;

namespace MagnaWms.Application.Roles.Commands.CreateRole;

public sealed record CreateRoleCommand(
    string Name,
    string Description,
    IReadOnlyList<string> PermissionKeys
) : IRequest<Result<RoleDto>>;
