using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.SalesOrders.Commands.CreateSalesOrder;
using MagnaWms.Application.SalesOrders.Commands.AllocateSalesOrder;
using MagnaWms.Application.SalesOrders.Commands.CancelSalesOrder;
using MagnaWms.Application.SalesOrders.Queries.GetSalesOrder;
using MagnaWms.Application.SalesOrders.Queries.GetSalesOrders;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using MagnaWms.Contracts.Sales;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Policy = Permissions.WarehousesManage)]
public sealed class SalesOrderController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _pdf;

    public SalesOrderController(IMediator mediator, MagnaProblemDetailsFactory pdf)
    {
        _mediator = mediator;
        _pdf = pdf;
    }

    /// <summary>Create a new sales order.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new sales order")]
    [SwaggerResponse(StatusCodes.Status200OK, "Sales order created.", typeof(SalesOrderDto))]
    public async Task<ActionResult<SalesOrderDto>> Create(
        CreateSalesOrderRequest request,
        CancellationToken ct)
    {
        Result<SalesOrderDto> result = await _mediator.Send(
            new CreateSalesOrderCommand(
                request.WarehouseId,
                request.OrderNumber,
                request.CustomerName,
                request.Lines),
            ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    /// <summary>Allocate a sales order.</summary>
    [HttpGet("{id:long}/allocate")]
    [SwaggerOperation(Summary = "Allocate a sales order")]
    [SwaggerResponse(StatusCodes.Status200OK, "Sales order allocated.", typeof(SalesOrderDto))]
    public async Task<ActionResult<SalesOrderDto>> Allocate(long id, CancellationToken ct)
    {
        Result<SalesOrderDto> result = await _mediator.Send(
            new AllocateSalesOrderCommand(id),
            ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    /// <summary>Cancel a sales order.</summary>
    [HttpGet("{id:long}/cancel")]
    [SwaggerOperation(Summary = "Cancel a sales order")]
    [SwaggerResponse(StatusCodes.Status200OK, "Sales order cancelled.")]
    public async Task<ActionResult> Cancel(long id, CancellationToken ct)
    {
        Result<Success> result = await _mediator.Send(
            new CancelSalesOrderCommand(id),
            ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    /// <summary>Get a sales order by ID.</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get sales order by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Sales order returned.", typeof(SalesOrderDto))]
    public async Task<ActionResult<SalesOrderDto>> GetById(long id, CancellationToken ct)
    {
        Result<SalesOrderDto> result = await _mediator.Send(
            new GetSalesOrderQuery(id),
            ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    /// <summary>Get all sales orders for a warehouse.</summary>
    [HttpGet("warehouse/{warehouseId:long}")]
    [SwaggerOperation(Summary = "Get sales orders for a warehouse")]
    [SwaggerResponse(StatusCodes.Status200OK, "Sales orders returned.", typeof(IReadOnlyList<SalesOrderDto>))]
    public async Task<ActionResult<IReadOnlyList<SalesOrderDto>>> GetByWarehouse(
        long warehouseId,
        CancellationToken ct)
    {
        Result<IReadOnlyList<SalesOrderDto>> result = await _mediator.Send(
            new GetSalesOrdersQuery(warehouseId),
            ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }
}
