using MediatR;
using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.UnitOfMeasures;

namespace MagnaWms.Application.UnitOfMeasures.Queries.GetUnitOfMeasureById;

public sealed record GetUnitOfMeasureByIdQuery(long UnitOfMeasureId) : IRequest<Result<UnitOfMeasureDto>>;
