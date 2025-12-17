using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Warehouses;
using MediatR;

namespace MagnaWms.Application.Warehouses.Command.DeactivateWarehouse;
public sealed record DeactivateWarehouseCommand(
    long WarehouseId
) : IRequest<Result<WarehouseDto>>;
