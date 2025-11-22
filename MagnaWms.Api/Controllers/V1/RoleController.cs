using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Roles.Commands.CreateRole;
using MagnaWms.Application.Roles.Commands.DeleteRole;
using MagnaWms.Application.Roles.Commands.UpdateRolePermissions;
using MagnaWms.Application.Roles.Queries.GetAllRoles;
using MagnaWms.Application.Roles.Queries.GetRoleById;
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
[Authorize(Policy = Permissions.SecurityManage)]
public sealed class RoleController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _magnaProblemDetailsFactory;

    public RoleController(IMediator mediator, MagnaProblemDetailsFactory magnaProblemDetailsFactory)
    {
        _mediator = mediator;
        _magnaProblemDetailsFactory = magnaProblemDetailsFactory;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all roles",
        Description = "Returns a list of all roles along with their associated permissions."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Roles retrieved successfully.", typeof(IReadOnlyList<RoleDto>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error.")]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAllAsync(CancellationToken ct)
    {
        Result<IReadOnlyList<RoleDto>> result = await _mediator.Send(new GetAllRolesQuery(), ct);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error));
    }

    [HttpPut("{id:long}/permissions")]
    [SwaggerOperation(
        Summary = "Update role permissions",
        Description = "Replaces all permissions assigned to a role with a new list of permission keys."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Role permissions updated successfully.", typeof(RoleDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Role not found.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error.")]
    public async Task<ActionResult<RoleDto>> UpdatePermissions(long id, [FromBody] UpdateRolePermissionsRequest request, CancellationToken ct)
    {
        Result<RoleDto> result = await _mediator.Send(
            new UpdateRolePermissionsCommand(id, request.PermissionKeys),
            ct);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error));
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new role",
        Description = "Creates a role with a name, optional description, and an initial list of permissions."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Role created successfully.", typeof(RoleDto))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Role already exists.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error.")]
    public async Task<ActionResult<RoleDto>> Create(
    [FromBody] CreateRoleRequest request,
    CancellationToken ct)
    {
        Result<RoleDto> result = await _mediator.Send(
            new CreateRoleCommand(request.Name, request.Description, request.PermissionKeys),
            ct);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error));
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(
        Summary = "Delete a role",
        Description = "Deletes a role only if it is not assigned to any users. Removes connections to associated permissions automatically."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Role deleted successfully.", typeof(Success))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Role not found.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Role is assigned to one or more users.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error.")]
    public async Task<ActionResult<Success>> Delete(long id, CancellationToken ct)
    {
        Result<Success> result = await _mediator.Send(new DeleteRoleCommand(id), ct);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error));
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get role by ID", Description = "Returns a single role with all assigned permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Role found.", typeof(RoleDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Role not found.")]
    public async Task<ActionResult<RoleDto>> GetById(long id, CancellationToken ct)
    {
        Result<RoleDto> result = await _mediator.Send(new GetRoleByIdQuery(id), ct);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error)
        );
    }
}
