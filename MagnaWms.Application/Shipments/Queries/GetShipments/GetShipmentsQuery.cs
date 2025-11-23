using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Shippings;
using MediatR;

namespace MagnaWms.Application.Shipments.Queries.GetShipments;

public sealed record GetShipmentsQuery(long WarehouseId)
    : IRequest<Result<IReadOnlyList<ShipmentDto>>>;
