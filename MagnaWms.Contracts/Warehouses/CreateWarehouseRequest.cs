namespace MagnaWms.Contracts.Warehouses;
public sealed record CreateWarehouseRequest(
    string Code,
    string Name,
    string Timezone
);
