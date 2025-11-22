using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Users.Commands.UpdateUserRoles;

public sealed record UpdateUserRolesCommand(
    long UserId,
    IReadOnlyList<long> RoleIds
) : IRequest<Result<UserDto>>;
