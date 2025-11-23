using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Putaway.Commands.ExecutePutaway;
using MagnaWms.Application.Putaway.Commands.CreatePutawayTask;
using MagnaWms.Contracts.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using MagnaWms.Contracts.Putaway;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Policy = Permissions.WarehousesManage)]
public sealed class PutawayController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _pdf;

    public PutawayController(IMediator mediator, MagnaProblemDetailsFactory pdf)
    {
        _mediator = mediator;
        _pdf = pdf;
    }

    /// <summary>
    /// Create a new putaway task.
    /// </summary>
    [HttpPost("task")]
    [SwaggerResponse(StatusCodes.Status200OK, "Putaway task created.", typeof(PutawayTaskDto))]
    public async Task<ActionResult<PutawayTaskDto>> CreateTask(
        CreatePutawayTaskRequest request,
        CancellationToken ct)
    {
        Result<PutawayTaskDto> result = await _mediator.Send(
            new CreatePutawayTaskCommand(
                request.ReceiptId,
                request.ReceiptLineId,
                request.QuantityToPutaway
            ), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    /// <summary>
    /// Execute an existing putaway task (move stock to final location).
    /// </summary>
    [HttpPost("task/{taskId:long}/execute")]
    [SwaggerResponse(StatusCodes.Status200OK, "Putaway executed.", typeof(PutawayTaskDto))]
    public async Task<ActionResult<PutawayTaskDto>> Execute(
        long taskId,
        ExecutePutawayRequest request,
        CancellationToken ct)
    {
        Result<PutawayTaskDto> result = await _mediator.Send(
            new ExecutePutawayCommand(
                taskId,
                request.Quantity,
                request.DestinationLocationId
            ), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }
}
