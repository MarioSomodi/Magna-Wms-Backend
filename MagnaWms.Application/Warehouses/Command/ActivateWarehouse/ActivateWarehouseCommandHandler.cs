using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Warehouses.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Warehouses;
using MagnaWms.Domain.WarehouseAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Warehouses.Command.ActivateWarehouse;
public sealed class ActivateWarehouseCommandHandler
    : IRequestHandler<ActivateWarehouseCommand, Result<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ActivateWarehouseCommandHandler(
        IWarehouseRepository warehouseRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<WarehouseDto>> Handle(
        ActivateWarehouseCommand request,
        CancellationToken cancellationToken)
    {
        Warehouse? warehouse = await _warehouseRepository
            .GetByIdAsync(request.WarehouseId, cancellationToken);

        if (warehouse is null)
        {
            return Result<WarehouseDto>.Failure(
                new Error(
                    ErrorCode.NotFound,
                    $"Warehouse with ID {request.WarehouseId} was not found."));
        }

        warehouse.Reactivate();

        _warehouseRepository.Update(warehouse);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        WarehouseDto dto = _mapper.Map<WarehouseDto>(warehouse);
        return Result<WarehouseDto>.Success(dto);
    }
}
