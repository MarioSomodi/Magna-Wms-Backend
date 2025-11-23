namespace MagnaWms.Contracts.Users;
public sealed record UserDto(
    long Id,
    string Email,
    bool IsActive,
    IReadOnlyList<string> Roles,
    IReadOnlyList<long> WarehouseIds,
    IReadOnlyList<string> Permissions
);
