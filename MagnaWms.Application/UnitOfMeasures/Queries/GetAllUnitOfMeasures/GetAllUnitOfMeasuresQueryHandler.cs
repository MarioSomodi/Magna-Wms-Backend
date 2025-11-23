using MediatR;
using MapsterMapper;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.UnitOfMeasures.Repository;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.UnitOfMeasureAggregate;
using MagnaWms.Contracts.UnitOfMeasures;

namespace MagnaWms.Application.UnitOfMeasures.Queries.GetAllUnitOfMeasures;

public sealed class GetAllUnitOfMeasuresQueryHandler
    : IRequestHandler<GetAllUnitOfMeasuresQuery, Result<IReadOnlyList<UnitOfMeasureDto>>>
{
    private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;
    private readonly IMapper _mapper;

    public GetAllUnitOfMeasuresQueryHandler(IUnitOfMeasureRepository unitOfMeasureRepository, IMapper mapper)
    {
        _unitOfMeasureRepository = unitOfMeasureRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<UnitOfMeasureDto>>> Handle(
        GetAllUnitOfMeasuresQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<UnitOfMeasure> unitOfMeasures = await _unitOfMeasureRepository.GetAllAsync(cancellationToken);

        if (unitOfMeasures.Count == 0)
        {
            return Result<IReadOnlyList<UnitOfMeasureDto>>.Failure(
                new Error(ErrorCode.NotFound, "No unit of measures found."));
        }

        IReadOnlyList<UnitOfMeasureDto> dtoList = _mapper.Map<IReadOnlyList<UnitOfMeasureDto>>(unitOfMeasures);
        return Result<IReadOnlyList<UnitOfMeasureDto>>.Success(dtoList);
    }
}
