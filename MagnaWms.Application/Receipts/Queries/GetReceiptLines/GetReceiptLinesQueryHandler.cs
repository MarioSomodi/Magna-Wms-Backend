using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Receipts.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Receipts;
using MagnaWms.Domain.ReceiptAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Receipts.Queries.GetReceiptLines;

public sealed class GetReceiptLinesQueryHandler
    : IRequestHandler<GetReceiptLinesQuery, Result<IReadOnlyList<ReceiptLineDto>>>
{
    private readonly IReceiptRepository _repo;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetReceiptLinesQueryHandler(
        IReceiptRepository repo,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _repo = repo;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<ReceiptLineDto>>> Handle(GetReceiptLinesQuery request, CancellationToken cancellationToken)
    {
        Receipt? receipt = await _repo.GetWithLinesAsync(request.ReceiptId, cancellationToken);

        if (receipt is null)
        {
            return Result<IReadOnlyList<ReceiptLineDto>>.Failure(
                new Error(ErrorCode.NotFound, "Receipt not found."));
        }

        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(receipt.WarehouseId))
        {
            return Result<IReadOnlyList<ReceiptLineDto>>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot access this receipt."));
        }

        var list = receipt.Lines.Select(l => _mapper.Map<ReceiptLineDto>(l)).ToList();

        return Result<IReadOnlyList<ReceiptLineDto>>.Success(list);
    }
}
