using MagnaWms.Application.Roles.Repository;
using MagnaWms.Domain.Authorization;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;

public sealed class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context) { }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        await Context.Set<Role>()
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);

    public async Task<IReadOnlyList<string>> GetPermissionKeysAsync(long roleId, CancellationToken cancellationToken = default)
        => await (
            from rp in Context.Set<RolePermission>()
            join p in Context.Set<Permission>() on rp.PermissionId equals p.Id
            where rp.RoleId == roleId
            select p.Key
        )
        .AsNoTracking()
        .Distinct()
        .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RolePermission>> GetRolePermissions(long roleId, CancellationToken cancellationToken = default)
    => await (
        from rp in Context.Set<RolePermission>()
        where rp.RoleId == roleId
        select rp
    )
    .ToListAsync(cancellationToken);

    public void RemoveRolePermissionsRange(IEnumerable<RolePermission> rolePermissions) => Context.Set<RolePermission>().RemoveRange(rolePermissions);
    public async Task AddRolePermissionsRangeAsync(IEnumerable<RolePermission> rolePermissions, CancellationToken cancellationToken) => await Context.Set<RolePermission>().AddRangeAsync(rolePermissions, cancellationToken);
}
