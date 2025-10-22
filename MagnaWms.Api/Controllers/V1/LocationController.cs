using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Api.Extensions;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Locations.Queries;
using MagnaWms.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class LocationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _magnaProblemDetailsFactory;

    public LocationController(IMediator mediator, MagnaProblemDetailsFactory magnaProblemDetailsFactory)
    {
        _mediator = mediator;
        _magnaProblemDetailsFactory = magnaProblemDetailsFactory;
    }

    /// <summary>
    /// Retrieves all locations for a specific warehouse.
    /// </summary>
    [HttpGet("{warehouseId:long}")]
    [SwaggerOperation(Summary = "Get locations by Warehouse ID", Description = "Returns all locations assigned to the specified warehouse.")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of locations returned successfully.", typeof(IReadOnlyList<LocationDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No locations found for the given warehouse.")]
    public async Task<ActionResult<IReadOnlyList<LocationDto>>> GetByWarehouseAsync(long warehouseId, CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<LocationDto>> result = await _mediator.Send(new GetLocationsByWarehouseIdQuery(warehouseId), cancellationToken);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error)
        );
    }
}
