using MediatR;
using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;

namespace MagnaWms.Application.UnitOfMeasures.Queries.GetUnitOfMeasureById;

public sealed record GetUnitOfMeasureByIdQuery(long UnitOfMeasureId) : IRequest<Result<UnitOfMeasureDto>>;
