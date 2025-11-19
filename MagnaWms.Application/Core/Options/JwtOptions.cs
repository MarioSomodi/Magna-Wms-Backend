namespace MagnaWms.Application.Core.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string? Secret { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationInMinutes { get; set; } = 15;
    public int RefreshTokenExpirationInDays { get; set; } = 7;
}
