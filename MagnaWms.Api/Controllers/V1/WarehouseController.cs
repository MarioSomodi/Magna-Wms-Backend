using MagnaWms.Contracts;
using MagnaWms.Application.Warehouses.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Swashbuckle.AspNetCore.Annotations;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class WarehouseController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehouseController(IMediator mediator) => _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    /// <summary>
    /// Retrieves all warehouses currently registered in the Magna WMS system.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a list of all active and inactive warehouses configured in the system.
    /// Each warehouse record contains:
    /// 
    /// * <b>WarehouseID</b> — Unique identifier  
    /// * <b>Code</b> — Internal warehouse code (e.g., `ZAG01`)  
    /// * <b>Name</b> — Display name of the warehouse  
    /// * <b>TimeZone</b> — Time zone associated with the warehouse  
    /// * <b>IsActive</b> — Indicates if the warehouse is active
    /// 
    /// Example usage:
    /// 
    ///     GET /api/v1/Warehouse
    /// 
    /// </remarks>
    /// <response code="200">Returns a list of all warehouses.</response>
    /// <response code="204">No warehouses found in the system.</response>
    /// <response code="500">An unexpected error occurred while retrieving data.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Retrieve all warehouses",
        Description = "Returns a list of all warehouses (active and inactive) registered in the Magna WMS system.")]
    [SwaggerResponse(StatusCodes.Status200OK, "A list of warehouses was successfully retrieved.", typeof(IReadOnlyList<WarehouseDto>))]
    [SwaggerResponse(StatusCodes.Status204NoContent, "No warehouses were found.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An internal server error occurred.")]
    public async Task<ActionResult<IReadOnlyList<WarehouseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<WarehouseDto> result = await _mediator.Send(new GetAllWarehousesQuery(), cancellationToken);
        return Ok(result);
    }
}
