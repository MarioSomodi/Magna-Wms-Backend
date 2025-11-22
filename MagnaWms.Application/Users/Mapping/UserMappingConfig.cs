using MagnaWms.Contracts;
using MagnaWms.Domain.UserAggregate;
using Mapster;

namespace MagnaWms.Application.Users.Mapping;
public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config) => config.NewConfig<(User user, IReadOnlyList<string> roles, IReadOnlyList<long> warehouses, IReadOnlyList<string> permissions), UserDto>()
            .Map(dest => dest.Id, src => src.user.Id)
            .Map(dest => dest.Email, src => src.user.Email)
            .Map(dest => dest.IsActive, src => src.user.IsActive)
            .Map(dest => dest.Roles, src => src.roles)
            .Map(dest => dest.WarehouseIds, src => src.warehouses)
            .Map(dest => dest.Permissions, src => src.permissions);
}
