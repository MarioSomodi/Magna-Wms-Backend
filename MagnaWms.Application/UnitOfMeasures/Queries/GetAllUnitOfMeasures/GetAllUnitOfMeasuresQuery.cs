using MediatR;
using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.UnitOfMeasures;

namespace MagnaWms.Application.UnitOfMeasures.Queries.GetAllUnitOfMeasures;

public sealed record GetAllUnitOfMeasuresQuery : IRequest<Result<IReadOnlyList<UnitOfMeasureDto>>>;
