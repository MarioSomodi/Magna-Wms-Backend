namespace MagnaWms.Contracts;

public sealed record WarehouseDto(
    long WarehouseID,
    string Code,
    string Name,
    string TimeZone,
    bool IsActive);
