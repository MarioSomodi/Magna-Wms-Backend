using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Warehouses;
using MediatR;

namespace MagnaWms.Application.Warehouses.Command.ActivateWarehouse;
public sealed record ActivateWarehouseCommand(
    long WarehouseId
) : IRequest<Result<WarehouseDto>>;
