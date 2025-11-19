using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Authentication;
using MediatR;

namespace MagnaWms.Application.Authentication.Command.Register;

public sealed record RegisterCommand(
    string Email,
    string Password
) : IRequest<Result<RegisterResponse>>;
