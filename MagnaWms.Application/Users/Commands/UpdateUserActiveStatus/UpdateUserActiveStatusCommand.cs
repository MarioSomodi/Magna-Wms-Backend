using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Users.Commands.UpdateUserActiveStatus;

public sealed record UpdateUserActiveStatusCommand(long UserId, bool IsActive)
    : IRequest<Result<UserDto>>;
