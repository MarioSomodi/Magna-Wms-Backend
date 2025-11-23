using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Users;
using MediatR;

namespace MagnaWms.Application.Authentication.Queires.GetCurrentUser;

public sealed record GetCurrentUserQuery() : IRequest<Result<UserDto>>;
