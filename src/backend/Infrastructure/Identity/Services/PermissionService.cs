using System.Security.Claims;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace EvrenDev.Infrastructure.Identity.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public PermissionService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<List<string>> GetUserPermissions(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = new HashSet<string>();

            foreach (var role in roles)
            {
                var rolePermissions = await GetRolePermissions(role);
                foreach (var permission in rolePermissions)
                {
                    permissions.Add(permission);
                }
            }

            return permissions.ToList();
        }

        public async Task<List<string>> GetRolePermissions(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return new List<string>();

            var claims = await _roleManager.GetClaimsAsync(role);
            return claims.Where(c => c.Type == "Permission")
                        .Select(c => c.Value)
                        .ToList();
        }

        public async Task<bool> HasPermission(string userId, string permission)
        {
            var permissions = await GetUserPermissions(userId);
            return permissions.Contains(permission);
        }

        public async Task<bool> HasAnyPermission(string userId, IEnumerable<string> permissions)
        {
            var userPermissions = await GetUserPermissions(userId);
            return permissions.Any(p => userPermissions.Contains(p));
        }

        public async Task<bool> HasAllPermissions(string userId, IEnumerable<string> permissions)
        {
            var userPermissions = await GetUserPermissions(userId);
            return permissions.All(p => userPermissions.Contains(p));
        }

        public async Task UpdateRolePermissions(string roleId, IEnumerable<string> permissions)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return;

            var claims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in claims.Where(c => c.Type == "Permission"))
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            foreach (var permission in permissions)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }
        }
    }
}
