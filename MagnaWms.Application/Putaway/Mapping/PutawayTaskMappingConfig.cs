using MagnaWms.Contracts.Putaway;
using MagnaWms.Domain.PutawayAggregate;
using Mapster;

namespace MagnaWms.Application.Putaway.Mapping;

public sealed class PutawayTaskMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config) => config.NewConfig<PutawayTask, PutawayTaskDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.WarehouseId, src => src.WarehouseId)
            .Map(dest => dest.ReceiptId, src => src.ReceiptId)
            .Map(dest => dest.ReceiptLineId, src => src.ReceiptLineId)
            .Map(dest => dest.ItemId, src => src.ItemId)
            .Map(dest => dest.QuantityToPutaway, src => src.QuantityToPutaway)
            .Map(dest => dest.QuantityCompleted, src => src.QuantityCompleted)
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.CreatedByUserId, src => src.CreatedByUserId)
            .Map(dest => dest.CompletedByUserId, src => src.CompletedByUserId)
            .Map(dest => dest.CreatedUtc, src => src.CreatedUtc)
            .Map(dest => dest.CompletedUtc, src => src.CompletedUtc);
}
