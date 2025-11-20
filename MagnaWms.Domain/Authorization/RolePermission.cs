using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.Authorization;

public sealed class RolePermission : Entity
{
    public long RoleId { get; private set; }
    public long PermissionId { get; private set; }

    private RolePermission() { }

    public RolePermission(long roleId, long permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}
