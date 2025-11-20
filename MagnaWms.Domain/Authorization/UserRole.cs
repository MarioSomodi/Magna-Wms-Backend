using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.Authorization;

public sealed class UserRole : Entity
{
    public long UserId { get; private set; }
    public long RoleId { get; private set; }

    private UserRole() { }

    public UserRole(long userId, long roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}
