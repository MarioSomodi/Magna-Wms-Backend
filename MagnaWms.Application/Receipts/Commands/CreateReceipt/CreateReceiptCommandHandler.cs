using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Receipts.Repository;
using MagnaWms.Application.Warehouses.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.ReceiptAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Receipts.Commands.CreateReceipt;
public sealed class CreateReceiptCommandHandler
    : IRequestHandler<CreateReceiptCommand, Result<ReceiptDto>>
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _uow;

    public CreateReceiptCommandHandler(
        IReceiptRepository receiptRepository,
        IWarehouseRepository warehouseRepository,
        ICurrentUser currentUser,
        IMapper mapper,
        IUnitOfWork uow)
    {
        _receiptRepository = receiptRepository;
        _warehouseRepository = warehouseRepository;
        _currentUser = currentUser;
        _mapper = mapper;
        _uow = uow;
    }

    public async Task<Result<ReceiptDto>> Handle(
        CreateReceiptCommand request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);
        if (!_currentUser.IsSuperAdmin && !allowed.Contains(request.WarehouseId))
        {
            return Result<ReceiptDto>.Failure(
                new Error(ErrorCode.Forbidden, "You are not allowed to create receipts for this warehouse."));
        }

        var receipt = new Receipt(
            request.WarehouseId,
            request.ReceiptNumber,
            request.ExternalReference,
            request.ExpectedArrivalDate,
            _currentUser.UserId!.Value
        );

        foreach (CreateReceiptLineRequest line in request.Lines)
        {
            receipt.AddLine(line.ItemId, line.ExpectedQty);
        }

        await _receiptRepository.AddAsync(receipt, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        ReceiptDto dto = _mapper.Map<ReceiptDto>(receipt);
        return Result<ReceiptDto>.Success(dto);
    }
}
