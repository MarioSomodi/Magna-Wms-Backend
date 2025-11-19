using MagnaWms.Api.Behaviors;
using MagnaWms.Application.Core.Errors;
using Microsoft.AspNetCore.Mvc;

namespace MagnaWms.Api.Controllers;

public static class ControllerExtensions
{
    public static ObjectResult ProblemResult(this ControllerBase controller, MagnaProblemDetailsFactory factory, Error error)
    {
        ProblemDetails problem = factory.CreateFromError(error);
        return controller.StatusCode(problem.Status ?? 500, problem);
    }
}
