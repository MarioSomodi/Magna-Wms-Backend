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

namespace MagnaWms.Application.Authentication.Command.Login;
public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _hasher;
    private readonly ITokenProvider _tokenProvider;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly JwtOptions _jwtOptions;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepo,
        IPasswordHasherService hasher,
        ITokenProvider tokenProvider,
        IRefreshTokenService refreshTokenService,
        IOptions<JwtOptions> jwtOptions,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepo;
        _hasher = hasher;
        _tokenProvider = tokenProvider;
        _refreshTokenService = refreshTokenService;
        _jwtOptions = jwtOptions.Value;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResult>> Handle(
        LoginCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return Result<LoginResult>.Failure(
                new Error(ErrorCode.Unauthorized, "Invalid credentials."));
        }

        if (!_hasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<LoginResult>.Failure(
                new Error(ErrorCode.Unauthorized, "Invalid credentials."));
        }

        string jwt = _tokenProvider.CreateAccessToken(user.Id, user.Email);

        string rawRefresh = _refreshTokenService.GenerateRefreshToken();
        string refreshHash = _refreshTokenService.HashToken(rawRefresh);

        var refreshToken = new RefreshToken(
            user.Id,
            refreshHash,
            DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationInDays),
            request.IpAddress);

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LoginResult>.Success(new LoginResult(user.Id, user.Email, jwt, rawRefresh));
    }
}
