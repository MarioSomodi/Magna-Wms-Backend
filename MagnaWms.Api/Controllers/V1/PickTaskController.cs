using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.PickTasks.Commands.CreatePickTask;
using MagnaWms.Application.PickTasks.Commands.PickItem;
using MagnaWms.Application.PickTasks.Commands.StartPickTask;
using MagnaWms.Application.PickTasks.Queries.GetPickTask;
using MagnaWms.Application.PickTasks.Queries.GetPickTasks;
using MagnaWms.Contracts.Authorization;
using MagnaWms.Contracts.Pick;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Policy = Permissions.WarehousesManage)]
public sealed class PickTaskController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _pdf;

    public PickTaskController(IMediator mediator, MagnaProblemDetailsFactory pdf)
    {
        _mediator = mediator;
        _pdf = pdf;
    }

    /// <summary>Create a pick task from a sales order</summary>
    [HttpPost("create")]
    [SwaggerOperation(Summary = "Create pick task")]
    public async Task<ActionResult<PickTaskDto>> Create(
        long salesOrderId,
        CancellationToken ct)
    {
        Result<PickTaskDto> result =
            await _mediator.Send(new CreatePickTaskCommand(salesOrderId), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    /// <summary>Start picking</summary>
    [HttpPost("{taskId:long}/start")]
    [SwaggerOperation(Summary = "Start pick task")]
    public async Task<ActionResult<PickTaskDto>> Start(long taskId, CancellationToken ct)
    {
        Result<PickTaskDto> result =
            await _mediator.Send(new StartPickTaskCommand(taskId), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    /// <summary>Pick a line</summary>
    [HttpPost("{taskId:long}/lines/{lineId:long}/pick")]
    [SwaggerOperation(Summary = "Pick item for a task line")]
    public async Task<ActionResult<PickTaskDto>> Pick(
        long taskId,
        long lineId,
        [FromBody] PickItemRequest request,
        CancellationToken ct)
    {
        Result<PickTaskDto> result = await _mediator.Send(
            new PickItemCommand(taskId, lineId, request.QuantityPicked), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    /// <summary>Get pick task by ID</summary>
    [HttpGet("{taskId:long}")]
    [SwaggerOperation(Summary = "Get pick task")]
    public async Task<ActionResult<PickTaskDto>> Get(long taskId, CancellationToken ct)
    {
        Result<PickTaskDto> result = await _mediator.Send(
            new GetPickTaskQuery(taskId), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    /// <summary>Get pick tasks for warehouse</summary>
    [HttpGet("warehouse/{warehouseId:long}")]
    [SwaggerOperation(Summary = "Get pick tasks by warehouse")]
    public async Task<ActionResult<IReadOnlyList<PickTaskDto>>> GetByWarehouse(
        long warehouseId,
        CancellationToken ct)
    {
        Result<IReadOnlyList<PickTaskDto>> result = await _mediator.Send(
            new GetPickTasksQuery(warehouseId), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }
}
