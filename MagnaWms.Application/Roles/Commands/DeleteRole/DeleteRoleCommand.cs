using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Roles.Commands.DeleteRole;

public sealed record DeleteRoleCommand(long RoleId) : IRequest<Result<Success>>;
