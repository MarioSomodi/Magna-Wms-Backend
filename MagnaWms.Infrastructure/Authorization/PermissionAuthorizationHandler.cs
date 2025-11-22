using Microsoft.AspNetCore.Authorization;

namespace MagnaWms.Infrastructure.Authorization;
internal sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Admin bypass
        if (context.User.Claims.Any(c => string.Equals(c.Type, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", StringComparison.OrdinalIgnoreCase) && string.Equals(c.Value, "SuperAdmin", StringComparison.OrdinalIgnoreCase)))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        bool hasPermission = context.User.Claims
            .Where(c => string.Equals(c.Type, "perm", StringComparison.OrdinalIgnoreCase))
            .Any(c => string.Equals(c.Value, requirement.Permission, StringComparison.OrdinalIgnoreCase));

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
