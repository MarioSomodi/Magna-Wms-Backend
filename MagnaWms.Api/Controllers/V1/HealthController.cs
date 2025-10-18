using Asp.Versioning;
using MagnaWms.Contracts.Errors;
using MagnaWms.Persistence;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class HealthController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public HealthController(AppDbContext dbContext) => _dbContext = dbContext;

    /// <summary>
    /// Returns a simple liveness indicator for the Magna WMS API.
    /// </summary>
    /// <remarks>
    /// This endpoint confirms the application is up and responding.
    /// Intended for human checks or monitoring tools.
    /// </remarks>
    /// <response code="200">The API is responding correctly.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Liveness check",
        Description = "Returns basic operational status of the Magna WMS API.")]
    [SwaggerResponse(StatusCodes.Status200OK, "The API is healthy.", typeof(object))]
    public IActionResult Get() => Ok(new { status = "ok" });

    /// <summary>
    /// Returns a static "pong" response to verify request/response pipeline integrity.
    /// </summary>
    /// <remarks>
    /// Use this to test connectivity, latency, or reverse proxy behavior.
    /// Often used by frontend health calls or monitoring scripts.
    /// </remarks>
    /// <response code="200">The API responded with 'pong'.</response>
    [HttpGet("ping")]
    [SwaggerOperation(
        Summary = "Ping test",
        Description = "Simple connectivity test endpoint. Returns 'pong' if reachable.")]
    [SwaggerResponse(StatusCodes.Status200OK, "API connectivity confirmed.", typeof(string))]
    public IActionResult Ping() => Ok("pong");

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

            return this.Problem(
                ErrorCode.DatabaseUnavailable,
                StatusCodes.Status503ServiceUnavailable,
                "The API could not connect to the database. Please check database health.");
        }
        catch (Exception ex)
        {
            return this.Problem(
                ErrorCode.Unknown,
                StatusCodes.Status503ServiceUnavailable,
                detail: ex.Message,
                title: "Readiness check failed");
        }
    }
}
