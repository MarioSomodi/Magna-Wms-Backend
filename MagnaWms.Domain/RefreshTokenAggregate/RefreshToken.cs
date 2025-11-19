using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.RefreshTokenAggregate;

public sealed class RefreshToken : AggregateRoot
{
    public long UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresUtc { get; private set; }
    public string CreatedByIp { get; private set; } = string.Empty;
    public DateTime? RevokedUtc { get; private set; }
    public string? RevokedByIp { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }

    private RefreshToken() { }

    public RefreshToken(long userId, string tokenHash, DateTime expires, string ip)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresUtc = expires;
        CreatedByIp = ip;
    }

    public bool IsActive => RevokedUtc is null && !IsExpired;
    public bool IsExpired => DateTime.UtcNow >= ExpiresUtc;

    public void Revoke(string ip, string? replacedByHash = null)
    {
        RevokedUtc = DateTime.UtcNow;
        RevokedByIp = ip;
        ReplacedByTokenHash = replacedByHash;
    }
}
