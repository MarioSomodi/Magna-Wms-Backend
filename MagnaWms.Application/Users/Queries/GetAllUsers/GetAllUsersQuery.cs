using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Users.Queries.GetAllUsers;

public sealed record GetAllUsersQuery : IRequest<Result<IReadOnlyList<UserDto>>>;
