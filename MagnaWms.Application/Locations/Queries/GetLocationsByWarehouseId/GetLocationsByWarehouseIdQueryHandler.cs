using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Locations.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.LocationAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Locations.Queries.GetLocationsByWarehouseId;
public sealed class GetLocationsByWarehouseIdQueryHandler
    : IRequestHandler<GetLocationsByWarehouseIdQuery, Result<IReadOnlyList<LocationDto>>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IMapper _mapper;

    public GetLocationsByWarehouseIdQueryHandler(ILocationRepository locationRepository, IMapper mapper)
    {
        _locationRepository = locationRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<LocationDto>>> Handle(
        GetLocationsByWarehouseIdQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Location> locations = await _locationRepository.GetByWarehouseIdAsync(request.WarehouseId, cancellationToken);

        if (locations.Count == 0)
        {
            return Result<IReadOnlyList<LocationDto>>.Failure(
                new Error(ErrorCode.NotFound, $"No locations found for Warehouse ID {request.WarehouseId}."));
        }

        IReadOnlyList<LocationDto> dtoList = _mapper.Map<IReadOnlyList<LocationDto>>(locations);
        return Result<IReadOnlyList<LocationDto>>.Success(dtoList);
    }
}
