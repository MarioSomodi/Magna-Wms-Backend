using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Abstractions.Forecasting;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Forecasting.Repository;
using MagnaWms.Application.Shipments.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.ForecastAggregate;
using MagnaWms.Domain.ShipmentAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Forecasting.Commands.TrainItemForecast;

public sealed class TrainItemForecastCommandHandler
    : IRequestHandler<TrainItemForecastCommand, Result<ForecastSeriesDto>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IForecastSeriesRepository _forecastSeriesRepository;
    private readonly IForecastingService _forecastingService;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TrainItemForecastCommandHandler(
        IShipmentRepository shipmentRepository,
        IForecastSeriesRepository forecastSeriesRepository,
        IForecastingService forecastingService,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _forecastSeriesRepository = forecastSeriesRepository;
        _forecastingService = forecastingService;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ForecastSeriesDto>> Handle(
        TrainItemForecastCommand request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(request.WarehouseId))
        {
            return Result<ForecastSeriesDto>.Failure(
                new Error(ErrorCode.Forbidden, "You are not allowed to train forecasts for this warehouse."));
        }

        IReadOnlyList<Shipment> shipments = await _shipmentRepository.GetByWarehouseWithLinesAsync(request.WarehouseId, cancellationToken);

        var history = shipments
            .SelectMany(s => s.Lines
                .Where(l => l.ItemId == request.ItemId)
                .Select(l => new { s.ShippedUtc, l.QuantityShipped }))
            .GroupBy(x => x.ShippedUtc.Date)
            .OrderBy(g => g.Key)
            .Select(g => (DateUtc: g.Key, Quantity: g.Sum(x => x.QuantityShipped)))
            .ToList();

        if (history.Count == 0)
        {
            return Result<ForecastSeriesDto>.Failure(
                new Error(ErrorCode.NotFound, "No shipment history found for this item in the specified warehouse."));
        }

        IReadOnlyList<(DateTime ForecastDateUtc, decimal Quantity)> forecastPoints = _forecastingService.ForecastDemand(history, request.HorizonDays);

        if (forecastPoints.Count == 0)
        {
            return Result<ForecastSeriesDto>.Failure(
                new Error(ErrorCode.BadRequest, "Insufficient history to produce a forecast."));
        }

        var series = new ForecastSeries(
            request.WarehouseId,
            request.ItemId,
            request.HorizonDays,
            modelInfo: "SSA");

        foreach ((DateTime forecastDateUtc, decimal quantity) in forecastPoints)
        {
            series.AddPoint(forecastDateUtc, quantity);
        }

        await _forecastSeriesRepository.AddAsync(series, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        ForecastSeriesDto dto = _mapper.Map<ForecastSeriesDto>(series);

        return Result<ForecastSeriesDto>.Success(dto);
    }
}
