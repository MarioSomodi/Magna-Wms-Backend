using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Warehouses.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Contracts.Warehouses;
using MagnaWms.Domain.WarehouseAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Warehouses.Command.CreateWarehouse;
public sealed class CreateWarehouseCommandHandler
    : IRequestHandler<CreateWarehouseCommand, Result<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateWarehouseCommandHandler(
        IWarehouseRepository warehouseRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<WarehouseDto>> Handle(
        CreateWarehouseCommand request,
        CancellationToken cancellationToken)
    {
        Warehouse? existingWarehouse = await _warehouseRepository.ExistsByCodeAsync(
            request.Code,
            cancellationToken);

        if (existingWarehouse != null)
        {
            return Result<WarehouseDto>.Failure(
                new Error(
                    ErrorCode.Conflict,
                    $"Warehouse code '{request.Code}' is already in use."));
        }

        var warehouse = new Warehouse(
            request.Code,
            request.Name,
            request.Timezone);

        await _warehouseRepository.AddAsync(warehouse, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        WarehouseDto dto = _mapper.Map<WarehouseDto>(warehouse);
        return Result<WarehouseDto>.Success(dto);
    }
}
