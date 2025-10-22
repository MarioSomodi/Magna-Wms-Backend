using MediatR;
using MapsterMapper;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.UnitOfMeasures.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.UnitOfMeasureAggregate;

namespace MagnaWms.Application.UnitOfMeasures.Queries.GetUnitOfMeasureById;

public sealed class GetUnitOfMeasureByIdQueryHandler
    : IRequestHandler<GetUnitOfMeasureByIdQuery, Result<UnitOfMeasureDto>>
{
    private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;
    private readonly IMapper _mapper;

    public GetUnitOfMeasureByIdQueryHandler(IUnitOfMeasureRepository unitOfMeasureRepository, IMapper mapper)
    {
        _unitOfMeasureRepository = unitOfMeasureRepository;
        _mapper = mapper;
    }

    public async Task<Result<UnitOfMeasureDto>> Handle(
        GetUnitOfMeasureByIdQuery request,
        CancellationToken cancellationToken)
    {
        UnitOfMeasure? uom = await _unitOfMeasureRepository.GetByIdAsync(request.UnitOfMeasureId, cancellationToken);

        if (uom is null)
        {
            return Result<UnitOfMeasureDto>.Failure(
                new Error(ErrorCode.NotFound, $"Unit of Measure with ID {request.UnitOfMeasureId} was not found."));
        }

        UnitOfMeasureDto dto = _mapper.Map<UnitOfMeasureDto>(uom);
        return Result<UnitOfMeasureDto>.Success(dto);
    }
}
