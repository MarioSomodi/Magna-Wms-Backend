using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Replenishment.Queries.GetReplenishmentSuggestionForItem;
using MagnaWms.Application.Replenishment.Queries.GetWarehouseSuggestions;
using MagnaWms.Contracts.Authorization;
using MagnaWms.Contracts.Replenishments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Policy = Permissions.WarehousesRead)]
public sealed class ReplenishmentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _pdf;

    public ReplenishmentController(IMediator mediator, MagnaProblemDetailsFactory pdf)
    {
        _mediator = mediator;
        _pdf = pdf;
    }

    /// <summary>
    /// Gets a replenishment suggestion for a specific item in a warehouse.
    /// </summary>
    [HttpGet("warehouse/{warehouseId:long}/item/{itemId:long}")]
    [SwaggerOperation(Summary = "Get replenishment suggestion for a single item")]
    [SwaggerResponse(StatusCodes.Status200OK, "Suggestion returned.", typeof(ReplenishmentSuggestionDto))] 
    public async Task<ActionResult<ReplenishmentSuggestionDto>> GetForItem(
        long warehouseId,
        long itemId,
        [FromQuery] int horizonDays = 30,
        CancellationToken ct = default)
    {
        Result<ReplenishmentSuggestionDto> result =
            await _mediator.Send(new GetReplenishmentSuggestionForItemQuery(warehouseId, itemId, horizonDays), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    /// <summary>
    /// Gets replenishment suggestions for all items in a warehouse.
    /// </summary>
    [HttpGet("warehouse/{warehouseId:long}")]
    [SwaggerOperation(Summary = "Get replenishment suggestions for all warehouse items")]
    [SwaggerResponse(StatusCodes.Status200OK, "Suggestions returned.", typeof(IReadOnlyList<ReplenishmentSuggestionDto>))]
    public async Task<ActionResult<IReadOnlyList<ReplenishmentSuggestionDto>>> GetForWarehouse(
        long warehouseId,
        [FromQuery] int horizonDays = 30,
        CancellationToken ct = default)
    {
        Result<IReadOnlyList<ReplenishmentSuggestionDto>> result =
            await _mediator.Send(new GetReplenishmentSuggestionsQuery(warehouseId, horizonDays), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }
}
