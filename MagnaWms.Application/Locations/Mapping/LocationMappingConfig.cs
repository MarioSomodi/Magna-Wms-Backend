using MagnaWms.Contracts.Locations;
using MagnaWms.Domain.LocationAggregate;
using Mapster;

namespace MagnaWms.Application.Locations.Mapping;
public class LocationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config) => config.NewConfig<Location, LocationDto>();
}
