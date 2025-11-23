using MagnaWms.Contracts.Inventories;
using MagnaWms.Domain.InventoryAggregate;
using Mapster;

namespace MagnaWms.Application.Inventories.Mapping;

public sealed class InventoryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config) => config.NewConfig<Inventory, InventoryDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.WarehouseId, src => src.WarehouseId)
            .Map(dest => dest.LocationId, src => src.LocationId)
            .Map(dest => dest.ItemId, src => src.ItemId)
            .Map(dest => dest.QuantityOnHand, src => src.QuantityOnHand)
            .Map(dest => dest.QuantityAllocated, src => src.QuantityAllocated)
            .Map(dest => dest.QuantityAvailable, src => src.QuantityAvailable);
}
