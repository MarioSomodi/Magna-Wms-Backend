using MagnaWms.Contracts.Authorization;
using MagnaWms.Domain.Authorization;
using Mapster;

namespace MagnaWms.Application.Roles.Mapping;
public sealed class RoleMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config) => config.NewConfig<(Role role, IReadOnlyList<string> perms), RoleDto>()
            .Map(dest => dest.Id, src => src.role.Id)
            .Map(dest => dest.Name, src => src.role.Name)
            .Map(dest => dest.Description, src => src.role.Description)
            .Map(dest => dest.Permissions, src => src.perms);
}
