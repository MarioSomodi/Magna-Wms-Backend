namespace MagnaWms.Contracts.Authorization;

public sealed record CreateRoleRequest(
    string Name,
    string Description,
    IReadOnlyList<string> PermissionKeys
);
