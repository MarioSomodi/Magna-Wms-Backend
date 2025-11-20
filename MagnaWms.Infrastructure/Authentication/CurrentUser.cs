using System.Globalization;
using System.Security.Claims;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Users.Repository;
using Microsoft.AspNetCore.Http;

namespace MagnaWms.Infrastructure.Authentication;

internal sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;


    public CurrentUser(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

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


    public bool IsInRole(string role) =>
        Principal?.IsInRole(role) == true
        || Principal?.Claims.Any(c =>
            string.Equals(c.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase)
            && string.Equals(c.Value, role, StringComparison.OrdinalIgnoreCase)) == true;

    public bool IsSuperAdmin => IsInRole("SuperAdmin");

    public async Task<IReadOnlyList<long>> GetAllowedWarehouses(CancellationToken cancellationToken) => UserId is not null ? await _userRepository.GetUsersWarehousesAsync(UserId.Value, cancellationToken) : [];
}
