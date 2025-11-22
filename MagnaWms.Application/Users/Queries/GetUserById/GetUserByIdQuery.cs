using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(long UserId)
    : IRequest<Result<UserDto>>;
