using MagnaWms.Contracts;
using MagnaWms.Domain.InventoryAggregate;
using Mapster;

namespace MagnaWms.Application.InventoryLedgers.Mapping;

public sealed class InventoryLedgerMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config) => config.NewConfig<InventoryLedgerEntry, InventoryLedgerEntryDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.WarehouseId, src => src.WarehouseId)
            .Map(dest => dest.LocationId, src => src.LocationId)
            .Map(dest => dest.ItemId, src => src.ItemId)
            .Map(dest => dest.TimestampUtc, src => src.TimestampUtc)
            .Map(dest => dest.QuantityChange, src => src.QuantityChange)
            .Map(dest => dest.ResultingQuantityOnHand, src => src.ResultingQuantityOnHand)
            .Map(dest => dest.MovementType, src => src.MovementType)
            .Map(dest => dest.ReferenceType, src => src.ReferenceType)
            .Map(dest => dest.ReferenceNumber, src => src.ReferenceNumber);
}
