using System.Security.Claims;

namespace EvrenDev.PublicApi.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    public string? TenantId => _httpContextAccessor.HttpContext?.User?.FindFirstValue("custom:tenantid");
    public string? FullName => _httpContextAccessor.HttpContext?.User?.FindFirstValue("custom:fullname");
    public string? FirstName => _httpContextAccessor.HttpContext?.User?.FindFirstValue("custom:firstname");
    public string? LastName => _httpContextAccessor.HttpContext?.User?.FindFirstValue("custom:lastname");
}
