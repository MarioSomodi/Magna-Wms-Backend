namespace MagnaWms.Application.Core.Abstractions.Authentication;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    long? UserId { get; }
    string? Email { get; }
}
