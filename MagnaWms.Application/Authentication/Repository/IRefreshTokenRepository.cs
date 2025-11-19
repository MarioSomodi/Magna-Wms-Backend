using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.RefreshTokenAggregate;

namespace MagnaWms.Application.Authentication.Repository;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> GetByHashAsync(string hash, CancellationToken cancellationToken);
}
