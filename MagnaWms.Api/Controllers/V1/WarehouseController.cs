using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Warehouses.Command.ActivateWarehouse;
using MagnaWms.Application.Warehouses.Command.CreateWarehouse;
using MagnaWms.Application.Warehouses.Command.DeactivateWarehouse;
using MagnaWms.Application.Warehouses.Command.UpdateWarehouse;
using MagnaWms.Application.Warehouses.Queries.GetAllWarehouses;
using MagnaWms.Application.Warehouses.Queries.GetWarehouseById;
using MagnaWms.Contracts.Authorization;
using MagnaWms.Contracts.Warehouses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class WarehouseController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _magnaProblemDetailsFactory;

    public WarehouseController(IMediator mediator, MagnaProblemDetailsFactory magnaProblemDetailsFactory) 
    {
        _mediator = mediator;
        _magnaProblemDetailsFactory = magnaProblemDetailsFactory;
    }

    /// <summary>
    /// Retrieves all warehouses.
    /// </summary>
    /// <response code="200">Warehouses retrieved successfully.</response>
    /// <response code="204">No warehouses found.</response>
    /// <response code="500">Unexpected server error.</response>
    [HttpGet]
    [Authorize(Policy = Permissions.WarehousesRead)]
    [SwaggerOperation(
        Summary = "Get all warehouses",
        Description = "Returns all warehouses configured in the Magna WMS system.")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of warehouses.", typeof(IReadOnlyList<WarehouseDto>))]
    [SwaggerResponse(StatusCodes.Status204NoContent, "No warehouses available.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error.")]
    public async Task<ActionResult<IReadOnlyList<WarehouseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<WarehouseDto>> result = await _mediator.Send(new GetAllWarehousesQuery(), cancellationToken);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error)
        );
    }

    /// <summary>
    /// Retrieves a warehouse by its ID.
    /// </summary>
    /// <response code="200">Warehouse found.</response>
    /// <response code="404">Warehouse not found.</response>
    /// <response code="500">Unexpected server error.</response>
    [HttpGet("{id:long}")]
    [Authorize(Policy = Permissions.WarehousesRead)]
    [SwaggerOperation(
        Summary = "Get warehouse by ID",
        Description = "Returns a specific warehouse by its unique identifier.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Warehouse found.", typeof(WarehouseDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Warehouse not found.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error.")]
    public async Task<ActionResult<WarehouseDto>> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        Result<WarehouseDto> result = await _mediator.Send(new GetWarehouseByIdQuery(id), cancellationToken);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error)
        );
    }

    [HttpPost]
    [Authorize(Policy = Permissions.WarehousesManage)]
    [SwaggerOperation(Summary = "Create a new warehouse")]
    [SwaggerResponse(StatusCodes.Status200OK, "Warehouse created.", typeof(WarehouseDto))]
    public async Task<ActionResult<WarehouseDto>> CreateAsync(
    [FromBody] CreateWarehouseRequest request,
    CancellationToken cancellationToken)
    {
        Result<WarehouseDto> result = await _mediator.Send(
            new CreateWarehouseCommand(
                request.Code,
                request.Name,
                request.Timezone),
            cancellationToken);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = Permissions.WarehousesManage)]
    [SwaggerOperation(Summary = "Update an existing warehouse")]
    [SwaggerResponse(StatusCodes.Status200OK, "Warehouse updated.", typeof(WarehouseDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Warehouse not found.")]
    public async Task<ActionResult<WarehouseDto>> UpdateAsync(
    long id,
    [FromBody] UpdateWarehouseRequest request,
    CancellationToken cancellationToken)
    {
        Result<WarehouseDto> result = await _mediator.Send(
            new UpdateWarehouseCommand(
                id,
                request.Name,
                request.Timezone),
            cancellationToken);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error));
    }

    [HttpGet("{id:long}/activate")]
    [Authorize(Policy = Permissions.WarehousesManage)]
    [SwaggerOperation(Summary = "Activate warehouse")]
    [SwaggerResponse(StatusCodes.Status200OK, "Warehouse activated.", typeof(WarehouseDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Warehouse not found.")]
    public async Task<ActionResult<WarehouseDto>> ActivateAsync(
    long id,
    CancellationToken cancellationToken)
    {
        Result<WarehouseDto> result = await _mediator.Send(
            new ActivateWarehouseCommand(id),
            cancellationToken);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error));
    }

    [HttpGet("{id:long}/deactivate")]
    [Authorize(Policy = Permissions.WarehousesManage)]
    [SwaggerOperation(Summary = "Deactivate warehouse")]
    [SwaggerResponse(StatusCodes.Status200OK, "Warehouse deactivated.", typeof(WarehouseDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Warehouse not found.")]
    public async Task<ActionResult<WarehouseDto>> DeactivateAsync(
        long id,
        CancellationToken cancellationToken)
    {
        Result<WarehouseDto> result = await _mediator.Send(
            new DeactivateWarehouseCommand(id),
            cancellationToken);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error));
    }
}
