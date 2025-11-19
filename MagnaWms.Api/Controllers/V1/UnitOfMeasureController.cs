using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.UnitOfMeasures.Queries.GetAllUnitOfMeasures;
using MagnaWms.Application.UnitOfMeasures.Queries.GetUnitOfMeasureById;
using MagnaWms.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class UnitOfMeasureController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _magnaProblemDetailsFactory;

    public UnitOfMeasureController(IMediator mediator, MagnaProblemDetailsFactory magnaProblemDetailsFactory)
    {
        _mediator = mediator;
        _magnaProblemDetailsFactory = magnaProblemDetailsFactory;
    }

    /// <summary>
    /// Retrieves all unit of measures.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all unit of measures", Description = "Returns all unit of measures in the system.")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of unit of measures returned successfully.", typeof(IReadOnlyList<UnitOfMeasureDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No unit of measures found.")]
    public async Task<ActionResult<IReadOnlyList<UnitOfMeasureDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<UnitOfMeasureDto>> result = await _mediator.Send(new GetAllUnitOfMeasuresQuery(), cancellationToken);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error)
        );
    }

    /// <summary>
    /// Retrieves a unit of measure by its ID.
    /// </summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get unit of measure by ID", Description = "Returns a unit of measure with the specified ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Unit of measure found.", typeof(UnitOfMeasureDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Unit of measure not found.")]
    public async Task<ActionResult<UnitOfMeasureDto>> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        Result<UnitOfMeasureDto> result = await _mediator.Send(new GetUnitOfMeasureByIdQuery(id), cancellationToken);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error)
        );
    }
}
