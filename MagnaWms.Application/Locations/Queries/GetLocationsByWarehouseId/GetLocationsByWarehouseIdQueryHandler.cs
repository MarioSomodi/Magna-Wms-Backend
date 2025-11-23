using System.Net.Http.Headers;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Locations.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Locations;
using MagnaWms.Domain.LocationAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Locations.Queries.GetLocationsByWarehouseId;
public sealed class GetLocationsByWarehouseIdQueryHandler
    : IRequestHandler<GetLocationsByWarehouseIdQuery, Result<IReadOnlyList<LocationDto>>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public GetLocationsByWarehouseIdQueryHandler(ILocationRepository locationRepository, IMapper mapper, ICurrentUser currentUser)
    {
        _locationRepository = locationRepository;
        _mapper = mapper;
        _currentUser = currentUser;
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

        IReadOnlyList<long> usersAllowedWarehouses = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!usersAllowedWarehouses.Contains(request.WarehouseId) && !_currentUser.IsSuperAdmin)
        {
            return Result<IReadOnlyList<LocationDto>>.Failure(
                    new Error(ErrorCode.Forbidden, "You are not allowed to access locations of this warehouse."));
        }

        IReadOnlyList<LocationDto> dtoList = _mapper.Map<IReadOnlyList<LocationDto>>(locations);
        return Result<IReadOnlyList<LocationDto>>.Success(dtoList);
    }
}
