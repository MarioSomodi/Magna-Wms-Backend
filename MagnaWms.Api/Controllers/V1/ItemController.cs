using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Items.Queries.GetAllItems;
using MagnaWms.Contracts.Items;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class ItemController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _magnaProblemDetailsFactory;

    public ItemController(IMediator mediator, MagnaProblemDetailsFactory magnaProblemDetailsFactory)
    {
        _mediator = mediator;
        _magnaProblemDetailsFactory = magnaProblemDetailsFactory;
    }

    /// <summary>
    /// Retrieves all items in the system.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all items", Description = "Returns all items currently registered in the system.")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of items returned successfully.", typeof(IReadOnlyList<ItemDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No items found.")]
    public async Task<ActionResult<IReadOnlyList<ItemDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<ItemDto>> result = await _mediator.Send(new GetAllItemsQuery(), cancellationToken);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error)
        );
    }
}
