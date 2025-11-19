namespace MagnaWms.Application.Core.Abstractions.Authentication;

public interface ITokenProvider
{
    string CreateAccessToken(long userId, string email);
}
