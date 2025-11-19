namespace MagnaWms.Contracts.Authentication;

public sealed record LoginResult(
    long UserId,
    string Email,
    string Jwt,
    string RefreshToken
);
