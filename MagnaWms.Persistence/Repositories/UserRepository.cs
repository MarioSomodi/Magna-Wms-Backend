using MagnaWms.Application.Users.Repository;
using MagnaWms.Domain.Authorization;
using MagnaWms.Domain.RefreshTokenAggregate;
using MagnaWms.Domain.UserAggregate;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;

public sealed class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) 
        => await Context.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<IReadOnlyList<string>> GetUsersPermissions(long userId, CancellationToken cancellationToken)
    => await (
            from ur in Context.Set<UserRole>()
            join rp in Context.Set<RolePermission>() on ur.RoleId equals rp.RoleId
            join p in Context.Set<Permission>() on rp.PermissionId equals p.Id
            where ur.UserId == userId
            select p.Key
        ).AsNoTracking().Distinct().ToListAsync(cancellationToken);
    public async Task<IReadOnlyList<string>> GetUsersRoles(long userId, CancellationToken cancellationToken)
    => await (
            from ur in Context.Set<UserRole>()
            join r in Context.Set<Role>() on ur.RoleId equals r.Id
            where ur.UserId == userId
            select r.Name
        ).AsNoTracking().Distinct().ToListAsync(cancellationToken);
    public async Task<IReadOnlyList<long>> GetUsersWarehousesAsync(long userId, CancellationToken cancellationToken) => await Context.Set<UserWarehouse>()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.WarehouseId)
            .ToListAsync(cancellationToken);

    public async Task SetUserRolesAsync(long userId, IReadOnlyList<long> roleIds, CancellationToken cancellationToken)
    {
        DbSet<UserRole> set = Context.Set<UserRole>();

        List<UserRole> existing = await set
            .Where(ur => ur.UserId == userId)
            .ToListAsync(cancellationToken);

        Context.RemoveRange(existing);

        if (roleIds.Count > 0)
        {
            IEnumerable<UserRole> toAdd = roleIds.Select(rid => new UserRole(userId, rid));
            await set.AddRangeAsync(toAdd, cancellationToken);
        }
    }

    public async Task SetUserWarehousesAsync(long userId, IReadOnlyList<long> warehouseIds, CancellationToken cancellationToken)
    {
        DbSet<UserWarehouse> set = Context.Set<UserWarehouse>();

        List<UserWarehouse> existing = await set
            .Where(uw => uw.UserId == userId)
            .ToListAsync(cancellationToken);

        Context.RemoveRange(existing);

        if (warehouseIds.Count > 0)
        {
            IEnumerable<UserWarehouse> toAdd = warehouseIds.Select(wid => new UserWarehouse(userId, wid));
            await set.AddRangeAsync(toAdd, cancellationToken);
        }
    }

    public async Task<bool> IsRoleAssignedToAnyUser(long roleId, CancellationToken cancellationToken) => await Context.Set<UserRole>()
            .AnyAsync(ur => ur.RoleId == roleId, cancellationToken);

    public async Task RemoveUserRolesAsync(long userId, CancellationToken cancellationToken)
    {
        DbSet<UserRole> set = Context.Set<UserRole>();
        List<UserRole> existing = await set.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        Context.RemoveRange(existing);
    }

    public async Task RemoveUserWarehousesAsync(long userId, CancellationToken cancellationToken)
    {
        DbSet<UserWarehouse> set = Context.Set<UserWarehouse>();
        List<UserWarehouse> existing = await set.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        Context.RemoveRange(existing);
    }

    public async Task RemoveRefreshTokensAsync(long userId, CancellationToken cancellationToken)
    {
        DbSet<RefreshToken> set = Context.Set<RefreshToken>();
        List<RefreshToken> existing = await set.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        Context.RemoveRange(existing);
    }

}
