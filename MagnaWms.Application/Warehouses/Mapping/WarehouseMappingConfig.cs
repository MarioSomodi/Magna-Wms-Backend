using Mapster;
using MagnaWms.Domain.WarehouseAggregate;
using MagnaWms.Contracts;

namespace MagnaWms.Application.Warehouses.Mapping;

public sealed class WarehouseMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config) => config.NewConfig<Warehouse, WarehouseDto>();
}
