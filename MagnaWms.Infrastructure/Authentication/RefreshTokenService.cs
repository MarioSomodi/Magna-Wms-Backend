using System.Security.Cryptography;
using MagnaWms.Application.Core.Abstractions.Authentication;

namespace MagnaWms.Infrastructure.Authentication;

internal sealed class RefreshTokenService : IRefreshTokenService
{
    public string GenerateRefreshToken()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public string HashToken(string token) => Convert.ToBase64String(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token)));
}
