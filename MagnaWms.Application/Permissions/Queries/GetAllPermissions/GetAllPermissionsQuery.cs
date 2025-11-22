using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Authorization;
using MediatR;

namespace MagnaWms.Application.Permissions.Queries.GetAllPermissions;

public sealed record GetAllPermissionsQuery
    : IRequest<Result<IReadOnlyList<PermissionDto>>>;
