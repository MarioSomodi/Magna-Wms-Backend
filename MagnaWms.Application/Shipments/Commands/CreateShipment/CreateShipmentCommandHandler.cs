using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.InventoryLedgers.Repository;
using MagnaWms.Application.SalesOrders.Repository;
using MagnaWms.Application.Shipments.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Shippings;
using MagnaWms.Domain.InventoryAggregate;
using MagnaWms.Domain.SalesOrderAggregate;
using MagnaWms.Domain.ShipmentAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Shipments.Commands.CreateShipment;

public sealed class CreateShipmentCommandHandler
    : IRequestHandler<CreateShipmentCommand, Result<ShipmentDto>>
{
    private readonly ISalesOrderRepository _orders;
    private readonly IShipmentRepository _shipments;
    private readonly IInventoryLedgerRepository _ledger;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _uow;

    public CreateShipmentCommandHandler(
        ISalesOrderRepository orders,
        IShipmentRepository shipments,
        IInventoryLedgerRepository ledger,
        ICurrentUser currentUser,
        IMapper mapper,
        IUnitOfWork uow)
    {
        _orders = orders;
        _shipments = shipments;
        _ledger = ledger;
        _currentUser = currentUser;
        _mapper = mapper;
        _uow = uow;
    }

    public async Task<Result<ShipmentDto>> Handle(
        CreateShipmentCommand request,
        CancellationToken cancellationToken)
    {
        SalesOrder? order =
            await _orders.GetWithLinesAsync(request.SalesOrderId, cancellationToken);

        if (order is null)
        {
            return Result<ShipmentDto>.Failure(
                new Error(ErrorCode.NotFound, "Sales order not found."));
        }

        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(order.WarehouseId))
        {
            return Result<ShipmentDto>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot ship from this warehouse."));
        }

        // Domain rule: must be fully picked
        if (!order.Lines.All(l => l.IsFullyPicked))
        {
            return Result<ShipmentDto>.Failure(
                new Error(ErrorCode.BadRequest, "Order is not fully picked."));
        }

        long userId = _currentUser.UserId!.Value;

        var shipment = new Shipment(
            order.WarehouseId,
            order.Id,
            request.ShipmentNumber,
            request.Carrier,
            request.TrackingNumber,
            userId
        );

        foreach (ShipmentLineRequest line in request.Lines)
        {
            shipment.AddLine(line.ItemId, line.Quantity);

            var ledger = new InventoryLedgerEntry(
                order.WarehouseId,
                locationId: 0,
                itemId: line.ItemId,
                quantityChange: 0,
                resultingQuantityOnHand: 0,
                movementType: "Shipment",
                referenceType: "SalesOrder",
                referenceNumber: order.OrderNumber
            );

            await _ledger.AddAsync(ledger, cancellationToken);
        }

        order.MarkShipped();

        await _shipments.AddAsync(shipment, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        ShipmentDto dto = _mapper.Map<ShipmentDto>(shipment);

        return Result<ShipmentDto>.Success(dto);
    }
}
