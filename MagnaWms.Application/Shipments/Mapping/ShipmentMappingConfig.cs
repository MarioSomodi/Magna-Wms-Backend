using MagnaWms.Contracts.Shippings;
using MagnaWms.Domain.ShipmentAggregate;
using Mapster;

namespace MagnaWms.Application.Shipments.Mapping;

public sealed class ShipmentMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ShipmentLine, ShipmentLineDto>()
            .Map(d => d.Id, s => s.Id)
            .Map(d => d.ItemId, s => s.ItemId)
            .Map(d => d.QuantityShipped, s => s.QuantityShipped);

        config.NewConfig<Shipment, ShipmentDto>()
            .Map(d => d.Id, s => s.Id)
            .Map(d => d.WarehouseId, s => s.WarehouseId)
            .Map(d => d.SalesOrderId, s => s.SalesOrderId)
            .Map(d => d.ShipmentNumber, s => s.ShipmentNumber)
            .Map(d => d.Carrier, s => s.Carrier)
            .Map(d => d.TrackingNumber, s => s.TrackingNumber)
            .Map(d => d.ShippedByUserId, s => s.ShippedByUserId)
            .Map(d => d.ShippedUtc, s => s.ShippedUtc)
            .Map(d => d.Lines, s => s.Lines);
    }
}
