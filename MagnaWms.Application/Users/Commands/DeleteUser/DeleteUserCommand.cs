using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Users.Commands.DeleteUser;

public sealed record DeleteUserCommand(long UserId) : IRequest<Result<Success>>;
