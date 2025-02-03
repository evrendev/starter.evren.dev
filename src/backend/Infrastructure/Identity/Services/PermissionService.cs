using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace EvrenDev.Infrastructure.Identity.Services;

public class PermissionService : IPermissionService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public PermissionService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<List<string>> GetUserPermissions(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return new List<string>();

        var claims = await _userManager.GetClaimsAsync(user);
        return claims.Where(c => c.Type == "permission")
                    .Select(c => c.Value)
                    .ToList();
    }

    public async Task<bool> HasPermission(Guid userId, string permission)
    {
        var permissions = await GetUserPermissions(userId);
        return permissions.Contains(permission);
    }

    public async Task<bool> HasAnyPermission(Guid userId, IEnumerable<string> permissions)
    {
        var userPermissions = await GetUserPermissions(userId);
        return permissions.Any(p => userPermissions.Contains(p));
    }

    public async Task<bool> HasAllPermissions(Guid userId, IEnumerable<string> permissions)
    {
        var userPermissions = await GetUserPermissions(userId);
        return permissions.All(p => userPermissions.Contains(p));
    }
}
