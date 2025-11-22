using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Inventories.Repository;
using MagnaWms.Application.InventoryLedgers.Repository;
using MagnaWms.Application.Locations.Repository;
using MagnaWms.Application.Receipts.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.InventoryAggregate;
using MagnaWms.Domain.ReceiptAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Receipts.Commands.ReceiveReceiptLine;

public sealed class ReceiveReceiptLineCommandHandler
    : IRequestHandler<ReceiveReceiptLineCommand, Result<ReceiptDto>>
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IInventoryLedgerRepository _ledgerRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ReceiveReceiptLineCommandHandler(
        IReceiptRepository receiptRepository,
        IInventoryRepository inventoryRepository,
        IInventoryLedgerRepository ledgerRepository,
        ILocationRepository locationRepository,
        ICurrentUser currentUser,
        IUnitOfWork uow,
        IMapper mapper)
    {
        _receiptRepository = receiptRepository;
        _inventoryRepository = inventoryRepository;
        _ledgerRepository = ledgerRepository;
        _locationRepository = locationRepository;
        _currentUser = currentUser;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<ReceiptDto>> Handle(
        ReceiveReceiptLineCommand request,
        CancellationToken cancellationToken)
    {
        // Load receipt with lines
        Receipt? receipt = await _receiptRepository.GetWithLinesAsync(request.ReceiptId, cancellationToken);

        if (receipt is null)
        {
            return Result<ReceiptDto>.Failure(
                new Error(ErrorCode.NotFound, "Receipt not found."));
        }

        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(receipt.WarehouseId))
        {
            return Result<ReceiptDto>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot receive inventory for this warehouse."));
        }

        bool locationOk = await _locationRepository.ExistsInWarehouse(
            request.ToLocationId,
            receipt.WarehouseId,
            cancellationToken);

        if (!locationOk)
        {
            return Result<ReceiptDto>.Failure(
                new Error(ErrorCode.BadRequest, "Invalid location for this warehouse."));
        }

        long userId = _currentUser.UserId!.Value;

        try
        {
            receipt.Receive(userId, request.LineId, request.Quantity);
        }
        catch (Exception ex)
        {
            return Result<ReceiptDto>.Failure(
                new Error(ErrorCode.BadRequest, ex.Message));
        }

        ReceiptLine line = receipt.Lines.First(l => l.Id == request.LineId);

        Inventory? inv = await _inventoryRepository.GetByKeyAsync(
            receipt.WarehouseId,
            request.ToLocationId,
            line.ItemId,
            cancellationToken);

        if (inv is null)
        {
            inv = new Inventory(
                receipt.WarehouseId,
                request.ToLocationId,
                line.ItemId,
                quantityOnHand: 0m);

            await _inventoryRepository.AddAsync(inv, cancellationToken);
        }

        inv.ApplyDelta(request.Quantity);

        var entry = new InventoryLedgerEntry(
            warehouseId: receipt.WarehouseId,
            locationId: request.ToLocationId,
            itemId: line.ItemId,
            quantityChange: request.Quantity,
            resultingQuantityOnHand: inv.QuantityOnHand,
            movementType: "Receiving",
            referenceType: "Receipt",
            referenceNumber: receipt.ReceiptNumber
        );

        await _ledgerRepository.AddAsync(entry, cancellationToken);

        await _uow.SaveChangesAsync(cancellationToken);

        ReceiptDto dto = _mapper.Map<ReceiptDto>(receipt);

        return Result<ReceiptDto>.Success(dto);
    }
}
