using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Inventories.Repository;
using MagnaWms.Application.InventoryLedgers.Repository;
using MagnaWms.Application.SalesOrders.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Sales;
using MagnaWms.Domain.InventoryAggregate;
using MagnaWms.Domain.SalesOrderAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.SalesOrders.Commands.AllocateSalesOrder;

public sealed class AllocateSalesOrderCommandHandler
    : IRequestHandler<AllocateSalesOrderCommand, Result<SalesOrderDto>>
{
    private readonly ISalesOrderRepository _orders;
    private readonly IInventoryRepository _inventory;
    private readonly IInventoryLedgerRepository _ledger;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AllocateSalesOrderCommandHandler(
        ISalesOrderRepository orders,
        IInventoryRepository inventory,
        IInventoryLedgerRepository ledger,
        ICurrentUser currentUser,
        IUnitOfWork uow,
        IMapper mapper)
    {
        _orders = orders;
        _inventory = inventory;
        _ledger = ledger;
        _currentUser = currentUser;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<SalesOrderDto>> Handle(
        AllocateSalesOrderCommand request,
        CancellationToken cancellationToken)
    {
        SalesOrder? order = await _orders.GetWithLinesAsync(request.SalesOrderId, cancellationToken);

        if (order is null)
        {
            return Result<SalesOrderDto>.Failure(
                new Error(ErrorCode.NotFound, "Sales order not found."));
        }

        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(order.WarehouseId))
        {
            return Result<SalesOrderDto>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot allocate orders for this warehouse."));
        }

        foreach (SalesOrderLine line in order.Lines)
        {
            Inventory? inv = (await _inventory.GetByWarehouseAsync(order.WarehouseId, cancellationToken)).FirstOrDefault(i => i.ItemId == line.ItemId);

            if (inv is null)
            {
                return Result<SalesOrderDto>.Failure(
                    new Error(ErrorCode.BadRequest,
                        $"No inventory found for item {line.ItemId}."));
            }

            try
            {
                inv.Allocate(line.QuantityOrdered);
            }
            catch (Exception ex)
            {
                return Result<SalesOrderDto>.Failure(
                    new Error(ErrorCode.BadRequest, ex.Message));
            }

            var entry = new InventoryLedgerEntry(
                order.WarehouseId,
                inv.LocationId,
                line.ItemId,
                0m,
                inv.QuantityOnHand,
                "Allocate",
                "SalesOrder",
                order.OrderNumber);

            await _ledger.AddAsync(entry, cancellationToken);

            order.AllocateLine(line.Id, line.QuantityOrdered);
        }

        await _uow.SaveChangesAsync(cancellationToken);

        SalesOrderDto dto = _mapper.Map<SalesOrderDto>(order);
        return Result<SalesOrderDto>.Success(dto);
    }
}
