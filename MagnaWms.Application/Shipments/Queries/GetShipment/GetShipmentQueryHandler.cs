using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Shipments.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Shippings;
using MagnaWms.Domain.ShipmentAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Shipments.Queries.GetShipment;

public sealed class GetShipmentQueryHandler
    : IRequestHandler<GetShipmentQuery, Result<ShipmentDto>>
{
    private readonly IShipmentRepository _repo;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetShipmentQueryHandler(
        IShipmentRepository repo,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _repo = repo;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<ShipmentDto>> Handle(GetShipmentQuery request, CancellationToken cancellationToken)
    {
        Shipment? shipment = await _repo.GetWithLinesAsync(request.ShipmentId, cancellationToken);

        if (shipment is null)
        {
            return Result<ShipmentDto>.Failure(
                new Error(ErrorCode.NotFound, "Shipment not found."));
        }

        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(shipment.WarehouseId))
        {
            return Result<ShipmentDto>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot view this shipment."));
        }

        ShipmentDto dto = _mapper.Map<ShipmentDto>(shipment);

        return Result<ShipmentDto>.Success(dto);
    }
}
