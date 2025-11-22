namespace MagnaWms.Contracts.Users;
public sealed record UpdateUserWarehousesRequest(
    IReadOnlyList<long> WarehouseIds
);
