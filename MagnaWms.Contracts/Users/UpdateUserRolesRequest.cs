namespace MagnaWms.Contracts.Users;
public sealed record UpdateUserRolesRequest(
    IReadOnlyList<long> RoleIds
);
