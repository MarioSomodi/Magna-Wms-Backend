namespace MagnaWms.Application.Core.Abstractions.Authentication;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    long? UserId { get; }
    string? Email { get; }
    bool IsSuperAdmin { get; }
    bool IsInRole(string role);
    Task<IReadOnlyList<long>> GetAllowedWarehouses(CancellationToken cancellationToken);
}
