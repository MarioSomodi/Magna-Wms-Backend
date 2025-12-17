using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Warehouses;
using MediatR;

namespace MagnaWms.Application.Warehouses.Command.UpdateWarehouse;
public sealed record UpdateWarehouseCommand(
    long WarehouseId,
    string Name,
    string Timezone
) : IRequest<Result<WarehouseDto>>;
