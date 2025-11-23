using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Putaway.Repository;
using MagnaWms.Application.Receipts.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.PutawayAggregate;
using MagnaWms.Domain.ReceiptAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Putaway.Commands.CreatePutawayTask;

public sealed class CreatePutawayTaskCommandHandler
    : IRequestHandler<CreatePutawayTaskCommand, Result<PutawayTaskDto>>
{
    private readonly IPutawayTaskRepository _repo;
    private readonly IReceiptRepository _receipts;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreatePutawayTaskCommandHandler(
        IPutawayTaskRepository repo,
        IReceiptRepository receipts,
        ICurrentUser currentUser,
        IUnitOfWork uow,
        IMapper mapper)
    {
        _repo = repo;
        _receipts = receipts;
        _currentUser = currentUser;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<PutawayTaskDto>> Handle(
        CreatePutawayTaskCommand request,
        CancellationToken cancellationToken)
    {
        Receipt? receipt = await _receipts.GetWithLinesAsync(request.ReceiptId, cancellationToken);

        if (receipt is null)
        {
            return Result<PutawayTaskDto>.Failure(
                new Error(ErrorCode.NotFound, "Receipt not found."));
        }

        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(receipt.WarehouseId))
        {
            return Result<PutawayTaskDto>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot create putaway tasks for this warehouse."));
        }

        ReceiptLine? line = receipt.Lines.FirstOrDefault(l => l.Id == request.ReceiptLineId);

        if (line is null)
        {
            return Result<PutawayTaskDto>.Failure(
                new Error(ErrorCode.BadRequest, "Receipt line does not exist."));
        }

        var task = new PutawayTask(
            receipt.WarehouseId,
            receipt.Id,
            line.Id,
            line.ItemId,
            request.QuantityToPutaway,
            _currentUser.UserId!.Value
        );

        await _repo.AddAsync(task, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        PutawayTaskDto dto = _mapper.Map<PutawayTaskDto>(task);

        return Result<PutawayTaskDto>.Success(dto);
    }
}
