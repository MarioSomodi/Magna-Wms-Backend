using MediatR;
using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Locations;

namespace MagnaWms.Application.Locations.Queries.GetLocationsByWarehouseId;

public sealed record GetLocationsByWarehouseIdQuery(long WarehouseId) : IRequest<Result<IReadOnlyList<LocationDto>>>;
