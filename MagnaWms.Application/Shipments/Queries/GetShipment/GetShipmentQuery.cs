using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Shippings;
using MediatR;

namespace MagnaWms.Application.Shipments.Queries.GetShipment;

public sealed record GetShipmentQuery(long ShipmentId)
    : IRequest<Result<ShipmentDto>>;
