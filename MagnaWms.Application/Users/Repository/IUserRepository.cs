using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.UserAggregate;

namespace MagnaWms.Application.Users.Repository;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
