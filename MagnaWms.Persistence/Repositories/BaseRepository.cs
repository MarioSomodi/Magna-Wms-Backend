using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Domain.Core.Primitives;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;

public class BaseRepository<T>(AppDbContext context) : IBaseRepository<T> where T : AggregateRoot
{
    protected AppDbContext Context { get; } = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await Context.Set<T>().FindAsync(new object[] { id }, cancellationToken);

    public async Task<T?> GetByIdNoTrackingAsync(long id, CancellationToken cancellationToken = default)
    => await Context.Set<T>()
        .AsNoTracking()
        .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await Context.Set<T>().AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await Context.Set<T>().AddAsync(entity, cancellationToken);

    public void Update(T entity) => Context.Set<T>().Update(entity);

    public void Remove(T entity) => Context.Set<T>().Remove(entity);
}
