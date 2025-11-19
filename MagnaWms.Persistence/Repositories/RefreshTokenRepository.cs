using MagnaWms.Application.Authentication.Repository;
using MagnaWms.Domain.RefreshTokenAggregate;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;

public sealed class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetByHashAsync(string hash, CancellationToken cancellationToken) => await Context.Set<RefreshToken>()
            .FirstOrDefaultAsync(x => x.TokenHash == hash, cancellationToken);
}
