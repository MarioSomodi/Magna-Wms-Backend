using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Users;
using MediatR;

namespace MagnaWms.Application.Users.Queries.GetAllUsers;

public sealed record GetAllUsersQuery : IRequest<Result<IReadOnlyList<UserDto>>>;
