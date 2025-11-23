using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Receipts.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Receipts;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Receipts.Queries.GetReceipts;

public sealed class GetReceiptsQueryHandler
    : IRequestHandler<GetReceiptsQuery, Result<IReadOnlyList<ReceiptDto>>>
{
    private readonly IReceiptRepository _repo;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetReceiptsQueryHandler(
        IReceiptRepository repo,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _repo = repo;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<ReceiptDto>>> Handle(GetReceiptsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<long> allowed = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowed.Contains(request.WarehouseId))
        {
            return Result<IReadOnlyList<ReceiptDto>>.Failure(
                new Error(ErrorCode.Forbidden, "You cannot access this warehouse."));
        }

        IReadOnlyList<Domain.ReceiptAggregate.Receipt> receipts = await _repo.GetByWarehouseIdAsync(request.WarehouseId, cancellationToken);

        var dtos = receipts.Select(r => _mapper.Map<ReceiptDto>(r)).ToList();

        return Result<IReadOnlyList<ReceiptDto>>.Success(dtos);
    }
}
