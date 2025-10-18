using MagnaWms.Contracts.Errors;
using Microsoft.AspNetCore.Mvc;

namespace MagnaWms.Api.Controllers;

public static class ControllerBaseExtensions
{
    /// <summary>
    /// Produce RFC7807 ProblemDetails with a typed error code.
    /// </summary>
    public static IActionResult Problem(this ControllerBase controller, ErrorCode code, int status, string? detail = null, string? title = null)
    {
        // Surface the code for the custom ProblemDetailsFactory
        controller.HttpContext.Items["errorCode"] = code.ToString();

        return controller.Problem(
            statusCode: status,
            title: title,
            detail: detail,
            type: $"https://magna-wms/errors/{code.ToString()
                                                  .ToUpperInvariant()}"
        );
    }
}
