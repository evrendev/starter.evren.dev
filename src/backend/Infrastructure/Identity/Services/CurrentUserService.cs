using System.Security.Claims;
using EvrenDev.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EvrenDev.Infrastructure.Identity.Services;

public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    public string? FullName => _httpContextAccessor.HttpContext?.User?.FindFirstValue("fullname") ?? string.Empty;
}
