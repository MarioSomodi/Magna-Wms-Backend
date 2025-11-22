using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.Authorization;

namespace MagnaWms.Application.Roles.Repository;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetPermissionKeysAsync(long roleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RolePermission>> GetRolePermissions(long roleId, CancellationToken cancellationToken = default);
    void RemoveRolePermissionsRange(IEnumerable<RolePermission> rolePermissions);
    Task AddRolePermissionsRangeAsync(IEnumerable<RolePermission> rolePermissions, CancellationToken cancellationToken);
}
