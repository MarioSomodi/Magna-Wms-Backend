using MagnaWms.Application.Authentication.Repository;
using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Options;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Users.Repository;
using MagnaWms.Contracts.Authentication;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.RefreshTokenAggregate;
using MagnaWms.Domain.UserAggregate;
using MediatR;
using Microsoft.Extensions.Options;

namespace MagnaWms.Application.Authentication.Command.Refresh;
public sealed class RefreshCommandHandler
    : IRequestHandler<RefreshCommand, Result<RefreshResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenProvider _tokenProvider;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly JwtOptions _jwtOptions;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshCommandHandler(
        IUserRepository userRepo,
        ITokenProvider tokenProvider,
        IRefreshTokenService refreshTokenService,
        IOptions<JwtOptions> jwtOptions,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepo;
        _tokenProvider = tokenProvider;
        _refreshTokenService = refreshTokenService;
        _jwtOptions = jwtOptions.Value;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RefreshResult>> Handle(
        RefreshCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RawRefreshToken))
        {
            return Result<RefreshResult>.Failure(new Error(ErrorCode.Unauthorized, "Refresh token not set"));
        }

        string refreshHash = _refreshTokenService.HashToken(request.RawRefreshToken);

        RefreshToken? storedRefreshToken = await _refreshTokenRepository.GetByHashAsync(refreshHash, cancellationToken);

        if (storedRefreshToken is null || !storedRefreshToken.IsActive)
        {
            return Result<RefreshResult>.Failure(new Error(ErrorCode.Unauthorized, "Refresh token invalid"));
        }

        User? user = await _userRepository.GetByIdAsync(storedRefreshToken.UserId, cancellationToken);
        
        if (user is null)
        {
            return Result<RefreshResult>.Failure(new Error(ErrorCode.Unauthorized, "Refresh token invalid"));
        }

        storedRefreshToken.Revoke(request.IpAddress);

        string newRefresh = _refreshTokenService.GenerateRefreshToken();
        string newRefreshHash = _refreshTokenService.HashToken(newRefresh);

        var replacement = new RefreshToken(
            storedRefreshToken.UserId,
            newRefreshHash,
            DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationInDays),
            request.IpAddress);

        _refreshTokenRepository.Update(storedRefreshToken);
        await _refreshTokenRepository.AddAsync(replacement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        string jwt = _tokenProvider.CreateAccessToken(user.Id, user.Email);

        return Result<RefreshResult>.Success(new RefreshResult(jwt, newRefresh));
    }
}
