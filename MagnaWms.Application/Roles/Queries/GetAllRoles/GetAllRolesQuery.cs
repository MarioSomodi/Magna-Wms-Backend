using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Authorization;
using MediatR;

namespace MagnaWms.Application.Roles.Queries.GetAllRoles;

public sealed record GetAllRolesQuery : IRequest<Result<IReadOnlyList<RoleDto>>>;
