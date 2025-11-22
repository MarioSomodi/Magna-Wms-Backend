using Mapster;
using MagnaWms.Contracts.Authorization;
using MagnaWms.Domain.Authorization;

namespace MagnaWms.Application.Permissions.Mapping;

public sealed class PermissionMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config) => config.NewConfig<Permission, PermissionDto>();
}
