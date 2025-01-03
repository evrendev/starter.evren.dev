using System.Security.Claims;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static EvrenDev.Shared.Constants.Policies;

namespace EvrenDev.Infrastructure.Identity.Seed;

public class IdentityDatabaseSeeder : IDatabaseSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IdentityDatabaseSeeder> _logger;

    public IdentityDatabaseSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
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
            // Add default roles
            var roles = new[] { "Admin", "User" };
            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                    _logger.LogInformation("Created role: {Role}", roleName);
                }
            }

            // Add permissions to admin role
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            if (adminRole != null)
            {
                _logger.LogInformation("Found Admin role with ID: {RoleId}", adminRole.Id);

                // Define admin permissions using constants
                var adminPermissions = AllModulesWithPermissions.ToList();

                // Get existing permissions
                var existingClaims = await _roleManager.GetClaimsAsync(adminRole);
                var existingPermissions = existingClaims
                    .Where(c => c.Type == "permission")
                    .Select(c => c.Value)
                    .ToList();

                _logger.LogInformation("Existing permissions for Admin role: {Permissions}",
                    string.Join(", ", existingPermissions));

                // Add missing permissions
                foreach (var permission in adminPermissions)
                {
                    if (!existingPermissions.Contains(permission))
                    {
                        var result = await _roleManager.AddClaimAsync(adminRole, new Claim("permission", permission));
                        if (result.Succeeded)
                        {
                            _logger.LogInformation("Added permission {Permission} to Admin role", permission);
                        }
                        else
                        {
                            _logger.LogError("Failed to add permission {Permission} to Admin role. Errors: {Errors}",
                                permission, string.Join(", ", result.Errors.Select(e => e.Description)));
                        }
                    }
                }

                // Verify permissions after adding
                var finalClaims = await _roleManager.GetClaimsAsync(adminRole);
                var finalPermissions = finalClaims
                    .Where(c => c.Type == "permission")
                    .Select(c => c.Value)
                    .ToList();

                _logger.LogInformation("Final permissions for Admin role: {Permissions}",
                    string.Join(", ", finalPermissions));
            }

            // Create default admin user if not exists
            var adminEmail = _configuration["DefaultAdmin:Email"] ?? "admin@example.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = _configuration["DefaultAdmin:FirstName"] ?? "System",
                    LastName = _configuration["DefaultAdmin:LastName"] ?? "Admin",
                    Deleted = false
                };

                var password = _configuration["DefaultAdmin:Password"] ?? "Admin123!";
                var result = await _userManager.CreateAsync(adminUser, password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Created admin user: {Email}", adminEmail);
                    if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
                    {
                        await _userManager.AddToRoleAsync(adminUser, "Admin");
                        _logger.LogInformation("Added admin user to Admin role");
                    }
                }
                else
                {
                    _logger.LogError("Failed to create admin user. Errors: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the identity database");
            throw;
        }
    }
}
