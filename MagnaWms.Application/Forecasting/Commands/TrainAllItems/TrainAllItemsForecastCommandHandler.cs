using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Abstractions.Forecasting;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Forecasting.Repository;
using MagnaWms.Application.Items.Repository;
using MagnaWms.Application.Shipments.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.ForecastAggregate;
using MagnaWms.Domain.ItemAggregate;
using MagnaWms.Domain.ShipmentAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Forecasting.Commands.TrainAllItemsForecast;

public sealed class TrainAllItemsForecastCommandHandler
    : IRequestHandler<TrainAllItemsForecastCommand, Result<TrainForecastResultDto>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IForecastSeriesRepository _seriesRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IForecastingService _forecastingService;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public TrainAllItemsForecastCommandHandler(
        IShipmentRepository shipmentRepository,
        IForecastSeriesRepository seriesRepository,
        IItemRepository itemRepository,
        IForecastingService forecastingService,
        ICurrentUser currentUser,
        IUnitOfWork uow,
        IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _seriesRepository = seriesRepository;
        _itemRepository = itemRepository;
        _forecastingService = forecastingService;
        _currentUser = currentUser;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<TrainForecastResultDto>> Handle(
        TrainAllItemsForecastCommand request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(request.WarehouseId))
        {
            return Result<TrainForecastResultDto>.Failure(
                new Error(ErrorCode.Forbidden, "You are not allowed to train forecasts for this warehouse."));
        }

        IReadOnlyList<Item> items = await _itemRepository.GetAllAsync(cancellationToken);
        int totalItems = items.Count;
        int forecasted = 0;
        int skipped = 0;

        IReadOnlyList<Shipment> shipments = await _shipmentRepository.GetByWarehouseWithLinesAsync(
            request.WarehouseId, cancellationToken);

        foreach (Item item in items)
        {
            var history = shipments
                .SelectMany(s => s.Lines
                    .Where(l => l.ItemId == item.Id)
                    .Select(l => new { s.ShippedUtc, l.QuantityShipped }))
                .GroupBy(x => x.ShippedUtc.Date)
                .OrderBy(g => g.Key)
                .Select(g => (DateUtc: g.Key, Quantity: g.Sum(x => x.QuantityShipped)))
                .ToList();

            if (history.Count == 0)
            {
                skipped++;
                continue;
            }

            IReadOnlyList<(DateTime ForecastDateUtc, decimal Quantity)> forecast = _forecastingService.ForecastDemand(history, request.HorizonDays);

            if (forecast.Count == 0)
            {
                skipped++;
                continue;
            }

            var series = new ForecastSeries(
                request.WarehouseId,
                item.Id,
                request.HorizonDays,
                "SSA");

            foreach ((DateTime ForecastDateUtc, decimal Quantity) in forecast)
            {
                series.AddPoint(ForecastDateUtc, Quantity);
            }

            await _seriesRepository.AddAsync(series, cancellationToken);
            forecasted++;
        }

        await _uow.SaveChangesAsync(cancellationToken);

        return Result<TrainForecastResultDto>.Success(
            new TrainForecastResultDto(
                request.WarehouseId,
                totalItems,
                forecasted,
                skipped
            ));
    }
}
