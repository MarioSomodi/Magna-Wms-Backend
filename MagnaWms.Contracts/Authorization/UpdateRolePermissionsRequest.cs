namespace MagnaWms.Contracts.Authorization;
public sealed record UpdateRolePermissionsRequest(
    IReadOnlyList<string> PermissionKeys
);
