using MagnaWms.Contracts;
using MagnaWms.Domain.ForecastAggregate;
using Mapster;

namespace MagnaWms.Application.Forecasting.Mapping;

public sealed class ForecastMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ForecastSeriesPoint, ForecastPointDto>()
            .Map(dest => dest.ForecastDateUtc, src => src.ForecastDateUtc)
            .Map(dest => dest.Quantity, src => src.Quantity);

        config.NewConfig<ForecastSeries, ForecastSeriesDto>()
            .Map(dest => dest.WarehouseId, src => src.WarehouseId)
            .Map(dest => dest.ItemId, src => src.ItemId)
            .Map(dest => dest.GeneratedUtc, src => src.CreatedUtc)
            .Map(dest => dest.HorizonDays, src => src.HorizonDays)
            .Map(dest => dest.ModelInfo, src => src.ModelInfo)
            .Map(dest => dest.Points, src => src.Points);
    }
}
