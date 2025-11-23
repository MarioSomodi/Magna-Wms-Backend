using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Shippings;
using MediatR;

namespace MagnaWms.Application.Shipments.Commands.CreateShipment;

public sealed record CreateShipmentCommand(
    long SalesOrderId,
    string ShipmentNumber,
    string Carrier,
    string TrackingNumber,
    IReadOnlyList<ShipmentLineRequest> Lines
) : IRequest<Result<ShipmentDto>>;
