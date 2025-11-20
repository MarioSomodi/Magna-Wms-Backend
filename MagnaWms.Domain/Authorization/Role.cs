using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.Authorization;

public sealed class Role : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    private readonly List<RolePermission> _permissions = [];
    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    private Role() { }

    public Role(string name, string? description = null)
    {
        Name = name;
        Description = description;
    }

    public void AddPermission(long permissionId) => _permissions.Add(new RolePermission(Id, permissionId));
}
