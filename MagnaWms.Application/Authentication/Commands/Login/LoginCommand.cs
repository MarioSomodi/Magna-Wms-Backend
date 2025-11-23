using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Authentication;
using MediatR;

namespace MagnaWms.Application.Authentication.Commands.Login;
public sealed record LoginCommand(string Email, string Password, string IpAddress)
    : IRequest<Result<LoginResult>>;
