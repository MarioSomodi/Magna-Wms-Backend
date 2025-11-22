namespace MagnaWms.Contracts.Authorization;

public sealed record PermissionDto(
    long Id,
    string Key,
    string Description
);
