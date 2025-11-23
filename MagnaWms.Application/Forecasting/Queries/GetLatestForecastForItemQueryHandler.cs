using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Forecasting.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.ForecastAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Forecasting.Queries.GetLatestForecastForItem;

public sealed class GetLatestForecastForItemQueryHandler
    : IRequestHandler<GetLatestForecastForItemQuery, Result<ForecastSeriesDto>>
{
    private readonly IForecastSeriesRepository _repo;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetLatestForecastForItemQueryHandler(
        IForecastSeriesRepository repo,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _repo = repo;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<ForecastSeriesDto>> Handle(GetLatestForecastForItemQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(request.WarehouseId))
        {
            return Result<ForecastSeriesDto>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot access this warehouse."));
        }

        ForecastSeries? series = await _repo.GetLatestForItemAsync(
            request.WarehouseId,
            request.ItemId,
            request.HorizonDays,
            cancellationToken);

        if (series is null)
        {
            return Result<ForecastSeriesDto>.Failure(
                new Error(ErrorCode.NotFound, "No forecast available."));
        }

        return Result<ForecastSeriesDto>.Success(_mapper.Map<ForecastSeriesDto>(series));
    }
}
