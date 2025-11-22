namespace MagnaWms.Contracts.Authorization;
public sealed record RoleDto(
    long Id,
    string Name,
    string Description,
    IReadOnlyList<string> Permissions
);
