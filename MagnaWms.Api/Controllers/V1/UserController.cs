using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Users.Commands.DeleteUser;
using MagnaWms.Application.Users.Commands.UpdateUserActiveStatus;
using MagnaWms.Application.Users.Commands.UpdateUserRoles;
using MagnaWms.Application.Users.Commands.UpdateUserWarehouses;
using MagnaWms.Application.Users.Queries.GetAllUsers;
using MagnaWms.Application.Users.Queries.GetUserById;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Authorization;
using MagnaWms.Contracts.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Policy = Permissions.SecurityManage)]
public sealed class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _magnaProblemDetailsFactory;

    public UserController(IMediator mediator, MagnaProblemDetailsFactory magnaProblemDetailsFactory)
    {
        _mediator = mediator;
        _magnaProblemDetailsFactory = magnaProblemDetailsFactory;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all users",
        Description = "Returns all users along with their assigned roles their permissions and allowed warehouses."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Users retrieved successfully.", typeof(IReadOnlyList<UserDto>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error.")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAllAsync(CancellationToken ct)
    {
        Result<IReadOnlyList<UserDto>> result = await _mediator.Send(new GetAllUsersQuery(), ct);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error));
    }

    [HttpPut("{id:long}/roles")]
    [SwaggerOperation(
        Summary = "Update user roles",
        Description = "Replaces the user's assigned roles with the specified list of role IDs."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "User roles updated successfully.", typeof(UserDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error.")]
    public async Task<ActionResult<UserDto>> UpdateRoles(long id, [FromBody] UpdateUserRolesRequest request, CancellationToken ct)
    {
        Result<UserDto> result = await _mediator.Send(
            new UpdateUserRolesCommand(id, request.RoleIds),
            ct);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error));
    }

    [HttpPut("{id:long}/warehouses")]
    [SwaggerOperation(
        Summary = "Update user warehouse access",
        Description = "Assigns the user access to the specified warehouse IDs."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "User warehouse access updated successfully.", typeof(UserDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error.")]
    public async Task<ActionResult<UserDto>> UpdateWarehouses(long id, [FromBody] UpdateUserWarehousesRequest request, CancellationToken ct)
    {
        Result<UserDto> result = await _mediator.Send(
            new UpdateUserWarehousesCommand(id, request.WarehouseIds),
            ct);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error));
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(
        Summary = "Delete a user",
        Description = "Deletes the specified user and removes all related role, warehouse, and refresh-token associations."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "User deleted successfully.", typeof(Success))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "You do not have permission to delete users.")]
    public async Task<ActionResult<Success>> Delete(long id, CancellationToken ct)
    {
        Result<Success> result = await _mediator.Send(new DeleteUserCommand(id), ct);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error)
        );
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get user by ID", Description = "Returns the user with full role, permissions and warehouse data.")]
    [SwaggerResponse(StatusCodes.Status200OK, "User found.", typeof(UserDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.")]
    public async Task<ActionResult<UserDto>> GetById(long id, CancellationToken ct)
    {
        Result<UserDto> result = await _mediator.Send(new GetUserByIdQuery(id), ct);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error)
        );
    }

    [HttpPut("{id:long}/active-status")]
    [SwaggerOperation(Summary = "Activate or deactivate a user")]
    [SwaggerResponse(StatusCodes.Status200OK, "User status updated.", typeof(UserDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.")]
    public async Task<ActionResult<UserDto>> UpdateActiveStatus(
    long id,
    [FromBody] UpdateUserActiveStatusRequest request,
    CancellationToken ct)
    {
        Result<UserDto> result = await _mediator.Send(
            new UpdateUserActiveStatusCommand(id, request.IsActive),
            ct);

        return result.Match(
            Ok,
            error => this.ProblemResult(_magnaProblemDetailsFactory, error)
        );
    }
}
