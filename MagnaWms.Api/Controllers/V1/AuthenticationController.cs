using System.Globalization;
using Asp.Versioning;
using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Authentication.Command.Login;
using MagnaWms.Application.Authentication.Command.Refresh;
using MagnaWms.Application.Authentication.Command.Register;
using MagnaWms.Application.Core.Options;
using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Authentication;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace MagnaWms.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly MagnaProblemDetailsFactory _magnaProblemDetailsFactory;
    private readonly JwtOptions _jwtOptions;

    public AuthenticationController(
        IMediator mediator,
        MagnaProblemDetailsFactory magnaProblemDetailsFactory,
        IOptions<JwtOptions> jwtOptions)
    {
        _mediator = mediator;
        _magnaProblemDetailsFactory = magnaProblemDetailsFactory;
        _jwtOptions = jwtOptions.Value;
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Authenticate user")]
    [SwaggerResponse(StatusCodes.Status200OK, "Login successful.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid credentials.")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        Result<LoginResult> result = await _mediator.Send(new LoginCommand(request.Email, request.Password, ip), ct);

        return result.Match(
            onSuccess: response =>
            {
                HttpContext.Response.Cookies.Append(
                    "auth",
                    response.Jwt,
                    getCookieOptions()
                );
                
                HttpContext.Response.Cookies.Append(
                    "refresh",
                    response.RefreshToken,
                    getCookieOptions(isForRefresh: true)
                );
                return Ok(new LoginResponse(response.UserId, response.Email));
            },
            onFailure: error => this.ProblemResult(_magnaProblemDetailsFactory, error)
        );
    }

    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register new user")]
    [SwaggerResponse(StatusCodes.Status200OK, "User registered successfully.")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Email already exists.")]
    public async Task<IActionResult> Register(RegisterCommand command, CancellationToken ct)
    {
        Result<RegisterResponse> result = await _mediator.Send(command, ct);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => this.ProblemResult(_magnaProblemDetailsFactory, error)
        );
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        string? rawRefresh = Request.Cookies["refresh"];
        string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        Result<RefreshResult> result = await _mediator.Send(new RefreshCommand(rawRefresh, ip), ct);

        return result.Match<RefreshResult, IActionResult>(
            onSuccess: response =>
            {
                HttpContext.Response.Cookies.Append("auth", response.Jwt, getCookieOptions());
                HttpContext.Response.Cookies.Append("refresh", response.RefreshToken, getCookieOptions(true));
                return Ok();
            },
            onFailure: error => this.ProblemResult(_magnaProblemDetailsFactory, error)
        );
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("auth");
        Response.Cookies.Delete("refresh");
        return Ok();
    }


    private CookieOptions getCookieOptions(bool isForRefresh = false) => new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = isForRefresh
            ? DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationInDays)
            : DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes),
        Path = "/"
    };

}
