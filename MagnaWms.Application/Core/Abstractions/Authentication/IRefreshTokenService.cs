namespace MagnaWms.Application.Core.Abstractions.Authentication;

public interface IRefreshTokenService
{
    string GenerateRefreshToken();
    string HashToken(string token);
}
