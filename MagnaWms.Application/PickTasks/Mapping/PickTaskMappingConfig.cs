using MagnaWms.Contracts.Pick;
using MagnaWms.Domain.PickTaskAggregate;
using Mapster;

namespace MagnaWms.Application.PickTasks.Mapping;

public sealed class PickTaskMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PickTaskLine, PickTaskLineDto>()
            .Map(d => d.Id, s => s.Id)
            .Map(d => d.ItemId, s => s.ItemId)
            .Map(d => d.LocationId, s => s.LocationId)
            .Map(d => d.QuantityToPick, s => s.QuantityToPick)
            .Map(d => d.QuantityPicked, s => s.QuantityPicked)
            .Map(d => d.IsCompleted, s => s.IsCompleted);

        config.NewConfig<PickTask, PickTaskDto>()
            .Map(d => d.Id, s => s.Id)
            .Map(d => d.WarehouseId, s => s.WarehouseId)
            .Map(d => d.SalesOrderId, s => s.SalesOrderId)
            .Map(d => d.Status, s => s.Status.ToString())
            .Map(d => d.CreatedUtc, s => s.CreatedUtc)
            .Map(d => d.UpdatedUtc, s => s.UpdatedUtc)
            .Map(d => d.CreatedByUserId, s => s.CreatedByUserId)
            .Map(d => d.CompletedByUserId, s => s.CompletedByUserId)
            .Map(d => d.CompletedUtc, s => s.CompletedUtc)
            .Map(d => d.Lines, s => s.Lines);
    }
}
