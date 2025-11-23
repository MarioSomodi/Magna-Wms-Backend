using MagnaWms.Contracts.Sales;
using MagnaWms.Domain.SalesOrderAggregate;
using Mapster;

namespace MagnaWms.Application.SalesOrders.Mapping;

public sealed class SalesOrderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<SalesOrderLine, SalesOrderLineDto>()
            .Map(d => d.Id, s => s.Id)
            .Map(d => d.ItemId, s => s.ItemId)
            .Map(d => d.QuantityOrdered, s => s.QuantityOrdered)
            .Map(d => d.QuantityAllocated, s => s.QuantityAllocated)
            .Map(d => d.QuantityPicked, s => s.QuantityPicked)
            .Map(d => d.IsFullyAllocated, s => s.IsFullyAllocated)
            .Map(d => d.IsFullyPicked, s => s.IsFullyPicked);

        config.NewConfig<SalesOrder, SalesOrderDto>()
            .Map(d => d.Id, s => s.Id)
            .Map(d => d.WarehouseId, s => s.WarehouseId)
            .Map(d => d.OrderNumber, s => s.OrderNumber)
            .Map(d => d.CustomerName, s => s.CustomerName)
            .Map(d => d.Status, s => s.Status.ToString())
            .Map(d => d.CreatedUtc, s => s.CreatedUtc)
            .Map(d => d.UpdatedUtc, s => s.UpdatedUtc)
            .Map(d => d.Lines, s => s.Lines);
    }
}
