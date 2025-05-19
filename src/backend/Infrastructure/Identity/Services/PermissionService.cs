using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.Identity.Services;

public class PermissionService : IPermissionService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(
        UserManager<ApplicationUser> userManager,
        ILogger<PermissionService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<List<string>> GetUserPermissions(Guid userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return new List<string>();

            var claims = await _userManager.GetClaimsAsync(user);
            return claims.Where(c => c.Type == "permission")
                        .Select(c => c.Value)
                        .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting user permissions.");
            return new List<string>();
        }
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
