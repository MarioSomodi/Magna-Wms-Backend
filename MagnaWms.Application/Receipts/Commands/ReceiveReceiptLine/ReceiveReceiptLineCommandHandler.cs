using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Inventories.Repository;
using MagnaWms.Application.InventoryLedgers.Repository;
using MagnaWms.Application.Locations.Repository;
using MagnaWms.Application.Receipts.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Receipts;
using MagnaWms.Domain.InventoryAggregate;
using MagnaWms.Domain.LocationAggregate;
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

        ReceiptLine? line = receipt.Lines.FirstOrDefault(l => l.Id == request.LineId);

        if (line is null)
        {
            return Result<ReceiptDto>.Failure(
                new Error(ErrorCode.BadRequest, "Receipt line not found."));
        }

        try
        {
            receipt.Receive(_currentUser.UserId!.Value, request.LineId, request.Quantity);
        }
        catch (Exception ex)
        {
            return Result<ReceiptDto>.Failure(
                new Error(ErrorCode.BadRequest, ex.Message));
        }

        IReadOnlyList<Location>? staging = await _locationRepository.GetStageLocationForWarehouse(receipt.WarehouseId, cancellationToken);
        if (staging is null)
        {
            return Result<ReceiptDto>.Failure(
                new Error(ErrorCode.BadRequest, "No staging location exists for this warehouse."));
        }

        Inventory? inv = await _inventoryRepository.GetByKeyAsync(
            receipt.WarehouseId,
            staging[0].Id,
            line.ItemId,
            cancellationToken);

        if (inv is null)
        {
            inv = new Inventory(
                receipt.WarehouseId,
                staging[0].Id,
                line.ItemId,
                quantityOnHand: 0m);

            await _inventoryRepository.AddAsync(inv, cancellationToken);
        }

        inv.ApplyDelta(request.Quantity);

        await _ledgerRepository.AddAsync(
            new InventoryLedgerEntry(
                warehouseId: receipt.WarehouseId,
                locationId: staging[0].Id,
                itemId: line.ItemId,
                quantityChange: request.Quantity,
                resultingQuantityOnHand: inv.QuantityOnHand,
                movementType: "Receiving",
                referenceType: "Receipt",
                referenceNumber: receipt.ReceiptNumber
            ),
            cancellationToken);

        await _uow.SaveChangesAsync(cancellationToken);

        ReceiptDto dto = _mapper.Map<ReceiptDto>(receipt);
        return Result<ReceiptDto>.Success(dto);
    }
}
