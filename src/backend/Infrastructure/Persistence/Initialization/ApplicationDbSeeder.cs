using EvrenDev.Domain.Identity;
using TenantInfo = EvrenDev.Domain.Multitenancy.TenantInfo;
using EvrenDev.Infrastructure.Identity;
using EvrenDev.Shared.Authorization;
using EvrenDev.Shared.Multitenancy;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.Persistence.Initialization;

// The concrete TenantInfo was directly DI-resolvable pre-v10 — no longer the case as of
// Finbuckle v10 (Task S3), read it through the accessor instead. Non-null-forgiving here
// (as the original ctor param was): this seeder only ever runs once DatabaseInitializer
// has explicitly set the tenant context for the scope it's resolved in.
internal class ApplicationDbSeeder(
    IMultiTenantContextAccessor<TenantInfo> multiTenantContextAccessor,
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    CustomSeederRunner seederRunner,
    ILogger<ApplicationDbSeeder> logger)
{
    private readonly TenantInfo currentTenant = multiTenantContextAccessor.MultiTenantContext!.TenantInfo!;

    public async Task SeedDatabaseAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        await SeedRolesAsync(dbContext);
        await SeedAdminUserAsync();
        await seederRunner.RunSeedersAsync(cancellationToken);
    }

    private async Task SeedRolesAsync(ApplicationDbContext dbContext)
    {
        await MigrateLegacyBasicRoleToStudentAsync();

        foreach (var roleName in ApiRoles.DefaultRoles)
        {
            if (await roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName)
                is not ApplicationRole role)
            {
                // Create the role
                logger.LogInformation("Seeding {role} Role for '{tenantId}' Tenant.", roleName, currentTenant.Id);
                role = new ApplicationRole(roleName, $"{roleName} Role for {currentTenant.Id} Tenant");
                await roleManager.CreateAsync(role);
            }

            switch (roleName)
            {
                // Assign permissions
                case ApiRoles.Student:
                    await AssignPermissionsToRoleAsync(dbContext, ApiPermissions.Basic, role);
                    await PruneStaleClaimsFromRoleAsync(dbContext, ApiPermissions.Basic, role);
                    break;
                case ApiRoles.Editor:
                    await AssignPermissionsToRoleAsync(dbContext, ApiPermissions.Editor, role);
                    break;
                case ApiRoles.Admin:
                    {
                        await AssignPermissionsToRoleAsync(dbContext, ApiPermissions.Admin, role);

                        if (currentTenant.Id == MultitenancyConstants.Root.Id)
                            await AssignPermissionsToRoleAsync(dbContext, ApiPermissions.Root, role);

                        break;
                    }
            }
        }
    }

    // One-time, self-healing migration: this tenant may still have the old
    // "Basic" role name (pre-Task-O1) and/or a manually-created, out-of-sync
    // "Student" role from the admin panel. Reconciles both into a single
    // canonical "Student" role without losing any user's role assignment.
    // No-op once "Basic" no longer exists (which it won't after the first run).
    private async Task MigrateLegacyBasicRoleToStudentAsync()
    {
        var legacyBasicRole = await roleManager.Roles.SingleOrDefaultAsync(r => r.Name == "Basic");
        if (legacyBasicRole is null)
            return;

        var staleStudentRole = await roleManager.Roles.SingleOrDefaultAsync(r => r.Name == ApiRoles.Student);
        if (staleStudentRole is not null)
        {
            // "Basic" (real seeded role, likely has real users + current claims) is
            // kept as the canonical row and simply renamed — its users stay attached
            // for free. The stale manually-created "Student" role is emptied first
            // so no user is orphaned, then deleted.
            logger.LogWarning(
                "Both 'Basic' and 'Student' roles exist for '{tenantId}' Tenant — merging into canonical 'Student'.",
                currentTenant.Id);

            foreach (var user in await userManager.GetUsersInRoleAsync(staleStudentRole.Name))
            {
                await userManager.AddToRoleAsync(user, legacyBasicRole.Name);
                await userManager.RemoveFromRoleAsync(user, staleStudentRole.Name);
            }

            await roleManager.DeleteAsync(staleStudentRole);
        }

        await roleManager.SetRoleNameAsync(legacyBasicRole, ApiRoles.Student);
        await roleManager.UpdateAsync(legacyBasicRole);
        logger.LogInformation("Renamed legacy 'Basic' role to '{role}' for '{tenantId}' Tenant.", ApiRoles.Student,
            currentTenant.Id);
    }

    // AssignPermissionsToRoleAsync is additive-only, so the pre-Task-O1 "Basic"
    // role's much wider claim set (full CRUD on Categories/Courses/Chapters/
    // Absences/Brands/Products, from before the IsBasic-only model existed)
    // survived the Basic -> Student rename untouched. That left Student with
    // real Create/Update/Delete permissions it was never meant to have — which
    // is what let the sidebar's "Course Management" section leak through even
    // after gating it on Create (Task O3). Student's permission set is fully
    // defined by ApiPermissions.Basic, so anything else on that specific role
    // is drift, not an intentional admin customization; prune it. Scoped to
    // Student only — Editor/Admin keep whatever an admin has configured for
    // them via the Roles UI.
    private async Task PruneStaleClaimsFromRoleAsync(ApplicationDbContext dbContext,
        IReadOnlyList<ApiPermission> permissions, ApplicationRole role)
    {
        var allowedValues = permissions.Select(p => p.Name).ToHashSet();
        var staleClaims = (await roleManager.GetClaimsAsync(role))
            .Where(c => c.Type == ApiClaims.Permission && !allowedValues.Contains(c.Value))
            .ToList();

        foreach (var claim in staleClaims)
        {
            logger.LogWarning(
                "Pruning stale '{claim}' Permission from {role} Role for '{tenantId}' Tenant.",
                claim.Value, role.Name, currentTenant.Id);
            await roleManager.RemoveClaimAsync(role, claim);
        }
    }

    private async Task AssignPermissionsToRoleAsync(ApplicationDbContext dbContext,
        IReadOnlyList<ApiPermission> permissions, ApplicationRole role)
    {
        var currentClaims = await roleManager.GetClaimsAsync(role);
        foreach (var permission in permissions)
        {
            if (!currentClaims.Any(c => c.Type == ApiClaims.Permission && c.Value == permission.Name))
            {
                logger.LogInformation("Seeding {role} Permission '{permission}' for '{tenantId}' Tenant.", role.Name,
                    permission.Name, currentTenant.Id);
                dbContext.RoleClaims.Add(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = ApiClaims.Permission,
                    ClaimValue = permission.Name,
                    CreatedBy = "ApplicationDbSeeder"
                });
                await dbContext.SaveChangesAsync();
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        if (string.IsNullOrWhiteSpace(currentTenant.Id) || string.IsNullOrWhiteSpace(currentTenant.AdminEmail))
            return;

        if (await userManager.Users.FirstOrDefaultAsync(u => u.Email == currentTenant.AdminEmail)
            is not ApplicationUser adminUser)
        {
            var adminUserName = $"{currentTenant.Id.Trim()}.{ApiRoles.Admin}".ToLowerInvariant();
            adminUser = new ApplicationUser
            {
                Gender = Gender.None,
                Language = Language.En,
                FirstName = "Admin",
                LastName = "User",
                Email = currentTenant.AdminEmail,
                UserName = currentTenant.AdminEmail,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            };

            logger.LogInformation("Seeding Default Admin User for '{tenantId}' Tenant.", currentTenant.Id);
            var password = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, MultitenancyConstants.DefaultPassword);
            await userManager.CreateAsync(adminUser);
        }

        // Assign role to user
        if (!await userManager.IsInRoleAsync(adminUser, ApiRoles.Admin))
        {
            logger.LogInformation("Assigning Admin Role to Admin User for '{tenantId}' Tenant.", currentTenant.Id);
            await userManager.AddToRoleAsync(adminUser, ApiRoles.Admin);
        }
    }
}
