using MagnaWms.Contracts.Items;
using MagnaWms.Domain.ItemAggregate;
using Mapster;

namespace MagnaWms.Application.Items.Mapping;
public class ItemMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config) => config.NewConfig<Item, ItemDto>();
}
