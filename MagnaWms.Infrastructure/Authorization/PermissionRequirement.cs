using Microsoft.AspNetCore.Authorization;

namespace MagnaWms.Infrastructure.Authorization;
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    public PermissionRequirement(string permission) => Permission = permission;
}
