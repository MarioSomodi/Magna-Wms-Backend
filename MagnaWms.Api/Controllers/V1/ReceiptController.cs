using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Receipts.Commands.CreateReceipt;
using MagnaWms.Application.Receipts.Commands.ReceiveReceiptLine;
using MagnaWms.Application.Receipts.Queries.GetReceipt;
using MagnaWms.Application.Receipts.Queries.GetReceiptLines;
using MagnaWms.Application.Receipts.Queries.GetReceipts;
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
[Authorize(Policy = Permissions.WarehousesRead)]
public sealed class ReceiptController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _pdf;

    public ReceiptController(IMediator mediator, MagnaProblemDetailsFactory pdf)
    {
        _mediator = mediator;
        _pdf = pdf;
    }

    [HttpPost]
    [Authorize(Policy = Permissions.WarehousesManage)]
    [SwaggerOperation(Summary = "Create a new receipt")]
    [SwaggerResponse(StatusCodes.Status200OK, "Receipt created.", typeof(ReceiptDto))]
    public async Task<ActionResult<ReceiptDto>> Create(CreateReceiptRequest request, CancellationToken ct)
    {
        Result<ReceiptDto> result = await _mediator.Send(
             new CreateReceiptCommand(
                 request.WarehouseId,
                 request.ReceiptNumber,
                 request.ExternalReference,
                 request.ExpectedArrivalDate,
                 request.Lines
             ), ct);


        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get receipt by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Receipt returned.", typeof(ReceiptDto))]
    public async Task<ActionResult<ReceiptDto>> GetById(long id, CancellationToken ct)
    {
        Result<ReceiptDto> result = await _mediator.Send(new GetReceiptQuery(id), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    [HttpGet("warehouse/{warehouseId:long}")]
    [SwaggerOperation(Summary = "Get all receipts for a warehouse")]
    [SwaggerResponse(StatusCodes.Status200OK, "Receipts returned.", typeof(IReadOnlyList<ReceiptDto>))]
    public async Task<ActionResult<IReadOnlyList<ReceiptDto>>> GetByWarehouse(long warehouseId, CancellationToken ct)
    {
        Result<IReadOnlyList<ReceiptDto>> result = await _mediator.Send(new GetReceiptsQuery(warehouseId), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    [HttpGet("{id:long}/lines")]
    [SwaggerOperation(Summary = "Get receipt lines for a receipt")]
    [SwaggerResponse(StatusCodes.Status200OK, "Receipt lines returned.", typeof(IReadOnlyList<ReceiptLineDto>))]
    public async Task<ActionResult<IReadOnlyList<ReceiptLineDto>>> GetLines(long id, CancellationToken ct)
    {
        Result<IReadOnlyList<ReceiptLineDto>> result = await _mediator.Send(new GetReceiptLinesQuery(id), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }
    [HttpPost("{id:long}/lines/{lineId:long}/receive")]
    [Authorize(Policy = Permissions.WarehousesManage)]
    [SwaggerOperation(Summary = "Receive quantity for a receipt line")]
    [SwaggerResponse(StatusCodes.Status200OK, "Receipt updated.", typeof(ReceiptDto))]
    public async Task<ActionResult<ReceiptDto>> ReceiveLine(
    long id,
    long lineId,
    [FromBody] ReceiveReceiptLineRequest request,
    CancellationToken ct)
    {
        var command = new ReceiveReceiptLineCommand(
            id,
            lineId,
            request.Quantity,
            request.ToLocationId,
            request.Notes
        );

        Result<ReceiptDto> result = await _mediator.Send(command, ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

}
