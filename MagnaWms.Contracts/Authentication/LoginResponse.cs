namespace MagnaWms.Contracts.Authentication;
public sealed record LoginResponse(
    long UserId,
    string Email
);

