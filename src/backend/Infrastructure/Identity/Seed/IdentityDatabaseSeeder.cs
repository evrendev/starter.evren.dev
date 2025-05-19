using System.Security.Claims;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Identity;
using EvrenDev.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static EvrenDev.Shared.Constants.Policies;

namespace EvrenDev.Infrastructure.Identity.Seed;

public class IdentityDatabaseSeeder : IDatabaseSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IdentityDatabaseSeeder> _logger;

    public IdentityDatabaseSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration configuration,
        ILogger<IdentityDatabaseSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            var roles = Roles.ToList;

            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var role = new ApplicationRole()
                    {
                        Name = roleName,
                        Description = roleName,
                        Deleted = false,
                    };

                    var response = await _roleManager.CreateAsync(role);

                    _logger.LogInformation("Created role: {Role}", roleName);
                }
            }

            // Create default admin user if not exists
            var adminEmail = _configuration["DefaultAdmin:Email"] ?? "help@help-dunya.org";
            var superAdminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (superAdminUser == null)
            {

                superAdminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = _configuration["DefaultAdmin:FirstName"] ?? "System",
                    LastName = _configuration["DefaultAdmin:LastName"] ?? "Admin",
                    Image = _configuration["DefaultAdmin:Image"] ?? null,
                    Deleted = false,
                    Language = Defaults.Language,
                };

                var password = _configuration["DefaultAdmin:Password"] ?? "P@s5w0rd.123";
                var result = await _userManager.CreateAsync(superAdminUser, password);

                if (result.Succeeded)
                {
                    // Assign roles to the admin user
                    await _userManager.AddToRolesAsync(superAdminUser, roles);
                    _logger.LogInformation("Created admin user: {Email}", adminEmail);
                }
                else
                {
                    _logger.LogError("Failed to create admin user. Errors: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    return;
                }
            }

            // Add permissions to admin user
            var superAdminPermissions = AllModulesWithPermissions.ToList();

            // Get existing permissions
            var existingClaims = await _userManager.GetClaimsAsync(superAdminUser);
            var existingPermissions = existingClaims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();

            _logger.LogInformation("Existing permissions for Admin user: {Permissions}",
                string.Join(", ", existingPermissions));

            // Add missing permissions
            foreach (var permission in superAdminPermissions)
            {
                if (!existingPermissions.Contains(permission))
                {
                    var result = await _userManager.AddClaimAsync(superAdminUser, new Claim("permission", permission));
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Added permission {Permission} to Admin user", permission);
                    }
                    else
                    {
                        _logger.LogError("Failed to add permission {Permission} to Admin user. Errors: {Errors}",
                            permission, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }

            // Verify permissions after adding
            var finalClaims = await _userManager.GetClaimsAsync(superAdminUser);
            var finalPermissions = finalClaims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();

            _logger.LogInformation("Final permissions for Admin user: {Permissions}",
                string.Join(", ", finalPermissions));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the identity database");
            throw;
        }
    }
}
