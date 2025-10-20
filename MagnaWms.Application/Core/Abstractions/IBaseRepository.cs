using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Application.Core.Abstractions;
public interface IBaseRepository<T> where T : AggregateRoot
{
    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<T?> GetByIdNoTrackingAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}
