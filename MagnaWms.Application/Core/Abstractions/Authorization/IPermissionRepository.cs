using MagnaWms.Domain.Authorization;

namespace MagnaWms.Application.Core.Abstractions.Authorization;
public interface IPermissionRepository
{
    Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Permission>> GetByKeysAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken = default);
}
