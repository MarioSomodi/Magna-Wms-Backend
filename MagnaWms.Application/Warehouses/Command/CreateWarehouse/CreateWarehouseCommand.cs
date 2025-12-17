using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Warehouses;
using MediatR;

namespace MagnaWms.Application.Warehouses.Command.CreateWarehouse;
public sealed record CreateWarehouseCommand(
    string Code,
    string Name,
    string Timezone
) : IRequest<Result<WarehouseDto>>;
