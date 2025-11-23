using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.InventoryLedgers.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Inventories;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.InventoryLedgers.Queries.GetInventoryLedger;

public sealed class GetInventoryLedgerQueryHandler
    : IRequestHandler<GetInventoryLedgerQuery, Result<IReadOnlyList<InventoryLedgerEntryDto>>>
{
    private readonly IInventoryLedgerRepository _ledgerRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetInventoryLedgerQueryHandler(
        IInventoryLedgerRepository ledgerRepository,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _ledgerRepository = ledgerRepository;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<InventoryLedgerEntryDto>>> Handle(
        GetInventoryLedgerQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<long> allowedWarehouses = await _currentUser.GetAllowedWarehouses(cancellationToken);

        if (!_currentUser.IsSuperAdmin && !allowedWarehouses.Contains(request.WarehouseId))
        {
            return Result<IReadOnlyList<InventoryLedgerEntryDto>>.Failure(
                new Error(ErrorCode.Forbidden, "You are not allowed to access this warehouse."));
        }

        IReadOnlyList<Domain.InventoryAggregate.InventoryLedgerEntry> entries = request.LocationId is not null
            ? await _ledgerRepository.GetForItemAtLocationAsync(
                request.WarehouseId,
                request.LocationId.Value,
                request.ItemId,
                cancellationToken)
            : await _ledgerRepository.GetForItemAsync(
                request.WarehouseId,
                request.ItemId,
                cancellationToken);

        IReadOnlyList<InventoryLedgerEntryDto> dtos =
            _mapper.Map<IReadOnlyList<InventoryLedgerEntryDto>>(entries);

        return Result<IReadOnlyList<InventoryLedgerEntryDto>>.Success(dtos);
    }
}
