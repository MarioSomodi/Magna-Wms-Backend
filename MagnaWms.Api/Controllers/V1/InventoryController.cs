using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Inventories.Queries.GetInventory;
using MagnaWms.Application.InventoryLedgers.Queries.GetInventoryLedger;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _magnaProblemDetailsFactory;

    public InventoryController(IMediator mediator, MagnaProblemDetailsFactory magnaProblemDetailsFactory)
    {
        _mediator = mediator;
        _magnaProblemDetailsFactory = magnaProblemDetailsFactory;
    }

    /// <summary>
    /// Returns inventory by warehouse and location.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = Permissions.InventoryRead)]
    [SwaggerOperation(
        Summary = "Get inventory",
        Description = "Returns inventory across allowed warehouses. Optionally filter by warehouse ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Inventory retrieved.", typeof(IReadOnlyList<InventoryDto>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User is not allowed to access requested warehouse.")]
    public async Task<ActionResult<IReadOnlyList<InventoryDto>>> GetAsync(
        [FromQuery] long? warehouseId,
        CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<InventoryDto>> result =
            await _mediator.Send(new GetInventoryQuery(warehouseId), cancellationToken);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error));
    }

    /// <summary>
    /// Returns ledger entries for an item.
    /// </summary>
    [HttpGet("{warehouseId:long}/items/{itemId:long}/ledger")]
    [Authorize(Policy = Permissions.InventoryLedgerRead)]
    [SwaggerOperation(
        Summary = "Get inventory ledger for item",
        Description = "Returns inventory ledger entries for a specific item in a warehouse. Optionally filter by location.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Ledger entries returned.", typeof(IReadOnlyList<InventoryLedgerEntryDto>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User is not allowed to access requested warehouse.")]
    public async Task<ActionResult<IReadOnlyList<InventoryLedgerEntryDto>>> GetLedgerAsync(
        long warehouseId,
        long itemId,
        [FromQuery] long? locationId,
        CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<InventoryLedgerEntryDto>> result =
            await _mediator.Send(new GetInventoryLedgerQuery(warehouseId, itemId, locationId), cancellationToken);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error));
    }
}
