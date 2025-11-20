using MagnaWms.Application.Users.Repository;
using MagnaWms.Domain.Authorization;
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
}
