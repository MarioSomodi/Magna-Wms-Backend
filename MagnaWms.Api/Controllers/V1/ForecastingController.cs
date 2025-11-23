using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Forecasting.Commands.TrainAllItemsForecast;
using MagnaWms.Application.Forecasting.Commands.TrainItemForecast;
using MagnaWms.Application.Forecasting.Queries.GetLatestForecastForItem;
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
public sealed class ForecastingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _pdf;

    public ForecastingController(IMediator mediator, MagnaProblemDetailsFactory pdf)
    {
        _mediator = mediator;
        _pdf = pdf;
    }

    /// <summary>
    /// Trains a demand forecast for a specific item in a warehouse.
    /// </summary>
    [HttpPost("warehouse/{warehouseId:long}/item/{itemId:long}/train")]
    [Authorize(Policy = Permissions.WarehousesManage)]
    [SwaggerOperation(
        Summary = "Train forecast for item",
        Description = "Trains an SSA-based demand forecast for the given warehouse and item.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Forecast series created.", typeof(ForecastSeriesDto))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User not allowed to access this warehouse.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No shipment history for this item.")]
    public async Task<ActionResult<ForecastSeriesDto>> Train(
        long warehouseId,
        long itemId,
        [FromQuery] int horizonDays = 30,
        CancellationToken ct = default)
    {
        Result<ForecastSeriesDto> result =
            await _mediator.Send(new TrainItemForecastCommand(warehouseId, itemId, horizonDays), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    /// <summary>
    /// Trains forecasts for all items in a warehouse.
    /// </summary>
    [HttpPost("warehouse/{warehouseId:long}/train-all")]
    [Authorize(Policy = Permissions.WarehousesManage)]
    [SwaggerOperation(Summary = "Train forecasts for all warehouse items",
        Description = "Aggregates shipment history for every item in a warehouse and trains SSA forecasts.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Training completed.", typeof(TrainForecastResultDto))]
    public async Task<ActionResult<TrainForecastResultDto>> TrainAll(
        long warehouseId,
        [FromQuery] int horizonDays = 30,
        CancellationToken ct = default)
    {
        Result<TrainForecastResultDto> result =
            await _mediator.Send(new TrainAllItemsForecastCommand(warehouseId, horizonDays), ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

    [HttpGet("warehouse/{warehouseId:long}/item/{itemId:long}/forecast")]
    [SwaggerOperation(Summary = "Get latest forecast for item")]
    [SwaggerResponse(StatusCodes.Status200OK, "Forecast returned.", typeof(ForecastSeriesDto))]
    public async Task<ActionResult<ForecastSeriesDto>> GetLatest(
    long warehouseId,
    long itemId,
    [FromQuery] int horizonDays = 30,
    CancellationToken ct = default)
    {
        Result<ForecastSeriesDto> result = await _mediator.Send(
            new GetLatestForecastForItemQuery(warehouseId, itemId, horizonDays),
            ct);

        return result.Match(Ok, e => this.ProblemResult(_pdf, e));
    }

}
