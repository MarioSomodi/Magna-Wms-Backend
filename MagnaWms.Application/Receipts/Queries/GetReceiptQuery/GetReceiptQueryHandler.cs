using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Receipts.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Receipts;
using MagnaWms.Domain.ReceiptAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Receipts.Queries.GetReceiptQuery;

public sealed class GetReceiptQueryHandler
    : IRequestHandler<GetReceiptQuery, Result<ReceiptDto>>
{
    private readonly IReceiptRepository _repo;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetReceiptQueryHandler(
        IReceiptRepository repo,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _repo = repo;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<ReceiptDto>> Handle(GetReceiptQuery request, CancellationToken cancellationToken)
    {
        Receipt? receipt = await _repo.GetByIdAsync(request.ReceiptId, cancellationToken);

        if (receipt is null)
        {
            return Result<ReceiptDto>.Failure(
                new Error(ErrorCode.NotFound, "Receipt not found."));
        }

        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(receipt.WarehouseId))
        {
            return Result<ReceiptDto>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot access this receipt."));
        }

        return Result<ReceiptDto>.Success(_mapper.Map<ReceiptDto>(receipt));
    }
}
