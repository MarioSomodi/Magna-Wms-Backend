using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Shipments.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Shippings;
using MagnaWms.Domain.ShipmentAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Shipments.Queries.GetShipments;

public sealed class GetShipmentsQueryHandler
    : IRequestHandler<GetShipmentsQuery, Result<IReadOnlyList<ShipmentDto>>>
{
    private readonly IShipmentRepository _repo;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetShipmentsQueryHandler(
        IShipmentRepository repo,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _repo = repo;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<ShipmentDto>>> Handle(GetShipmentsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(request.WarehouseId))
        {
            return Result<IReadOnlyList<ShipmentDto>>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot view this warehouse's shipments."));
        }

        IReadOnlyList<Shipment> shipments = await _repo.GetByWarehouseAsync(request.WarehouseId, cancellationToken);
        var dtos = shipments.Select(s => _mapper.Map<ShipmentDto>(s)).ToList();

        return Result<IReadOnlyList<ShipmentDto>>.Success(dtos);
    }
}
