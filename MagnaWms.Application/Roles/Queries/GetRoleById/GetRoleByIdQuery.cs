using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Authorization;
using MediatR;

namespace MagnaWms.Application.Roles.Queries.GetRoleById;

public sealed record GetRoleByIdQuery(long RoleId)
    : IRequest<Result<RoleDto>>;
