using MagnaWms.Application.Core.Abstractions.Authorization;
using MagnaWms.Domain.Authorization;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;

public sealed class PermissionRepository(AppDbContext context) : IPermissionRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Set<Permission>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Permission>> GetByKeysAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken = default) =>
        await _context.Set<Permission>()
            .Where(p => keys.Contains(p.Key))
            .ToListAsync(cancellationToken);
}
