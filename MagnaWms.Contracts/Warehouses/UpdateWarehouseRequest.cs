namespace MagnaWms.Contracts.Warehouses;
public sealed record UpdateWarehouseRequest(
    string Name,
    string Timezone
);
