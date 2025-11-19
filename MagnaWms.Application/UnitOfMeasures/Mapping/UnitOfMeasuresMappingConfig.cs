using MagnaWms.Contracts;
using MagnaWms.Domain.UnitOfMeasureAggregate;
using Mapster;

namespace MagnaWms.Application.UnitOfMeasures.Mapping;

public sealed class UnitOfMeasuresMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config) => config.NewConfig<UnitOfMeasure, UnitOfMeasureDto>();
}
