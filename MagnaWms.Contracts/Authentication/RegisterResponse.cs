namespace MagnaWms.Contracts.Authentication;

public sealed record RegisterResponse(
    long UserId,
    string Email
);
