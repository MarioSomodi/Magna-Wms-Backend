using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.UserAggregate;

namespace MagnaWms.Application.Users.Repository;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> GetUsersPermissions(long userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> GetUsersRoles(long userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<long>> GetUsersWarehousesAsync(long userId, CancellationToken cancellationToken);
    Task SetUserRolesAsync(long userId, IReadOnlyList<long> roleIds, CancellationToken cancellationToken);
    Task SetUserWarehousesAsync(long userId, IReadOnlyList<long> warehouseIds, CancellationToken cancellationToken);
    Task<bool> IsRoleAssignedToAnyUser(long roleId, CancellationToken cancellationToken);
    Task RemoveUserRolesAsync(long userId, CancellationToken cancellationToken);
    Task RemoveUserWarehousesAsync(long userId, CancellationToken cancellationToken);
    Task RemoveRefreshTokensAsync(long userId, CancellationToken cancellationToken);
}
