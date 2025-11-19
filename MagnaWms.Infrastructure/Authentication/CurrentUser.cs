using System.Globalization;
using System.Security.Claims;
using MagnaWms.Application.Core.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace MagnaWms.Infrastructure.Authentication;

internal sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public long? UserId
    {
        get
        {
            ClaimsPrincipal? principal = _httpContextAccessor.HttpContext?.User;
            string? id = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            return long.TryParse(id, CultureInfo.InvariantCulture, out long parsed) ? parsed : null;
        }
    }

    public string? Email =>
        _httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.Email);
}
