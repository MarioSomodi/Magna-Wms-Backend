using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Shipments.Commands.CreateShipment;
using MagnaWms.Application.Shipments.Queries.GetShipment;
using MagnaWms.Application.Shipments.Queries.GetShipments;
using MagnaWms.Contracts.Authorization;
using MagnaWms.Contracts.Shippings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Policy = Permissions.WarehousesManage)]
public sealed class ShipmentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _pdf;

    public ShipmentController(IMediator mediator, MagnaProblemDetailsFactory pdf)
    {
        _mediator = mediator;
        _pdf = pdf;
    }

    /// <summary>
    /// Creates a new shipment for a sales order.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create shipment")]
    [SwaggerResponse(StatusCodes.Status200OK, "Shipment created.", typeof(ShipmentDto))]
    public async Task<ActionResult<ShipmentDto>> Create(CreateShipmentRequest request, CancellationToken ct)
    {
        Result<ShipmentDto> result = await _mediator.Send(
            new CreateShipmentCommand(
                request.SalesOrderId,
                request.ShipmentNumber,
                request.Carrier,
                request.TrackingNumber,
                request.Lines
            ), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    /// <summary>
    /// Returns a shipment by ID.
    /// </summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get shipment")]
    [SwaggerResponse(StatusCodes.Status200OK, "Shipment returned.", typeof(ShipmentDto))]
    public async Task<ActionResult<ShipmentDto>> Get(long id, CancellationToken ct)
    {
        Result<ShipmentDto> result = await _mediator.Send(new GetShipmentQuery(id), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    /// <summary>
    /// Returns all shipments for a warehouse.
    /// </summary>
    [HttpGet("warehouse/{warehouseId:long}")]
    [SwaggerOperation(Summary = "Get shipments for warehouse")]
    [SwaggerResponse(StatusCodes.Status200OK, "Shipments returned.", typeof(IReadOnlyList<ShipmentDto>))]
    public async Task<ActionResult<IReadOnlyList<ShipmentDto>>> GetByWarehouse(long warehouseId, CancellationToken ct)
    {
        Result<IReadOnlyList<ShipmentDto>> result =
            await _mediator.Send(new GetShipmentsQuery(warehouseId), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }
}
