using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Contracts.Errors;
using MagnaWms.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class HealthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly MagnaProblemDetailsFactory _magnaProblemDetailsFactory;

    public HealthController(AppDbContext dbContext, MagnaProblemDetailsFactory magnaProblemDetailsFactory)
    {
        _dbContext = dbContext;
        _magnaProblemDetailsFactory = magnaProblemDetailsFactory;
    }

    /// <summary>
    /// Returns a simple liveness indicator for the Magna WMS API.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Liveness check", Description = "Returns basic operational status of the Magna WMS API.")]
    [SwaggerResponse(StatusCodes.Status200OK, "The API is healthy.", typeof(object))]
    public IActionResult Get() => Ok(new { status = "ok" });

    /// <summary>
    /// Returns a static "pong" response to verify request/response pipeline integrity.
    /// </summary>
    [HttpGet("ping")]
    [SwaggerOperation(Summary = "Ping test", Description = "Simple connectivity test endpoint. Returns 'pong' if reachable.")]
    [SwaggerResponse(StatusCodes.Status200OK, "API connectivity confirmed.", typeof(string))]
    public IActionResult Ping() => Ok("pong");

    /// <summary>
    /// Checks database connectivity and API readiness.
    /// </summary>
    [HttpGet("ready")]
    [SwaggerOperation(Summary = "Readiness check", Description = "Verifies DB connectivity and overall API readiness.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Application and database are ready.")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Database unavailable.")]
    public async Task<IActionResult> Ready(CancellationToken cancellationToken)
    {
        try
        {
            if (await _dbContext.Database.CanConnectAsync(cancellationToken))
            {
                return Ok(new { status = "ready" });
            }

            var error = new Error(ErrorCode.DatabaseUnavailable, "The API could not connect to the database.");
            return this.ProblemResult(_magnaProblemDetailsFactory, error);
        }
        catch (Exception ex)
        {
            var error = new Error(ErrorCode.InternalError, ex.Message);
            return this.ProblemResult(_magnaProblemDetailsFactory, error);
        }
    }
}
