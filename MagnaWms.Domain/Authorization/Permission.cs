using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.Authorization;

public sealed class Permission : AggregateRoot
{
    public string Key { get; private set; } = null!;
    public string? Description { get; private set; }

    private Permission() { }

    public Permission(string key, string? description = null)
    {
        Key = key;
        Description = description;
    }
}
