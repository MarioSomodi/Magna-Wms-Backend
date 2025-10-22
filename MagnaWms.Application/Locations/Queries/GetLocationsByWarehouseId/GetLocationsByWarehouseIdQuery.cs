using MediatR;
using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;

namespace MagnaWms.Application.Locations.Queries;

public sealed record GetLocationsByWarehouseIdQuery(long WarehouseId) : IRequest<Result<IReadOnlyList<LocationDto>>>;
