using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Authentication;
using MediatR;

namespace MagnaWms.Application.Authentication.Command.Refresh;
public sealed record RefreshCommand(string? RawRefreshToken, string IpAddress)
    : IRequest<Result<RefreshResult>>;
