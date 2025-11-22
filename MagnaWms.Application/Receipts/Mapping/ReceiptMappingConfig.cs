using MagnaWms.Contracts;
using MagnaWms.Domain.ReceiptAggregate;
using Mapster;

namespace MagnaWms.Application.Receipts.Mapping;

public class ReceiptMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ReceiptLine, ReceiptLineDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.ItemId, src => src.ItemId)
            .Map(dest => dest.ItemSku, src => src.ItemSku)
            .Map(dest => dest.ItemName, src => src.ItemName)
            .Map(dest => dest.UnitOfMeasure, src => src.UnitOfMeasure)
            .Map(dest => dest.ExpectedQty, src => src.ExpectedQuantity)
            .Map(dest => dest.ReceivedQty, src => src.ReceivedQuantity)
            .Map(dest => dest.ToLocationId, src => src.ToLocationId)
            .Map(dest => dest.Notes, src => src.Notes)
            .Map(dest => dest.IsFullyReceived, src => src.IsFullyReceived);

        config.NewConfig<Receipt, ReceiptDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.WarehouseId, src => src.WarehouseId)
            .Map(dest => dest.ReceiptNumber, src => src.ReceiptNumber)
            .Map(dest => dest.ExternalReference, src => src.ExternalReference)
            .Map(dest => dest.ExpectedArrivalDate, src => src.ExpectedArrivalDate)
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.CreatedUtc, src => src.CreatedUtc)
            .Map(dest => dest.ClosedUtc, src => src.ClosedUtc)
            .Map(dest => dest.CreatedByUserId, src => src.CreatedByUserId)
            .Map(dest => dest.ReceivedByUserId, src => src.ReceivedByUserId)
            .Map(dest => dest.Lines, src => src.Lines);
    }
}
