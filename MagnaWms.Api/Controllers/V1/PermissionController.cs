using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Permissions.Queries.GetAllPermissions;
using MagnaWms.Contracts.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Policy = Permissions.SecurityManage)]
public sealed class PermissionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _magnaProblemDetailsFactory;

    public PermissionController(IMediator mediator, MagnaProblemDetailsFactory magnaProblemDetailsFactory)
    {
        _mediator = mediator;
        _magnaProblemDetailsFactory = magnaProblemDetailsFactory;
    }

    /// <summary>
    /// Retrieves all available permissions in the system.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all permissions")]
    [SwaggerResponse(StatusCodes.Status200OK, "Permissions retrieved successfully.", typeof(IReadOnlyList<PermissionDto>))]
    public async Task<ActionResult<IReadOnlyList<PermissionDto>>> GetAllAsync(CancellationToken ct)
    {
        Result<IReadOnlyList<PermissionDto>> result = await _mediator.Send(new GetAllPermissionsQuery(), ct);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error)
        );
    }
}
