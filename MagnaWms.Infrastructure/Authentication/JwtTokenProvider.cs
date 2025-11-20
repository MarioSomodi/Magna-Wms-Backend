using System.Globalization;
using System.Security.Claims;
using System.Text;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Options;
using MagnaWms.Contracts.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace MagnaWms.Infrastructure.Authentication;

internal sealed class JwtTokenProvider : ITokenProvider
{
    private readonly IConfiguration _configuration;
    private readonly JwtOptions _jwtOptions;

    public JwtTokenProvider(IConfiguration configuration, IOptions<JwtOptions> jwtOptions) { 
        _configuration = configuration;
        _jwtOptions = jwtOptions.Value;
    }

    public string CreateAccessToken(long userId, string email, IEnumerable<string> permissions, IEnumerable<string> roles)
    {
        string secretKey = _jwtOptions.Secret
            ?? throw new InvalidOperationException("Jwt:Secret is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString(CultureInfo.InvariantCulture)),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture))
        ];

        foreach (string perm in permissions)
        {
            claims = [.. claims, new Claim("perm", perm)];
        }

        foreach (string role in roles)
        {
            claims = [.. claims, new Claim("role", role)];
        }

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience
        };

        var handler = new JsonWebTokenHandler();

        return handler.CreateToken(descriptor);
    }
}
