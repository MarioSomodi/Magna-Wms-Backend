using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Users;
using MediatR;

namespace MagnaWms.Application.Users.Commands.UpdateUserWarehouses;

public sealed record UpdateUserWarehousesCommand(
    long UserId,
    IReadOnlyList<long> WarehouseIds
) : IRequest<Result<UserDto>>;
