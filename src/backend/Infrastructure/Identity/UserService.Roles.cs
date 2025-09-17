using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Identity.Users.Entities;
using EvrenDev.Application.Identity.Users.Queries.UserRoles;
using EvrenDev.Domain.Common.Events.Identity;
using EvrenDev.Shared.Authorization;
using EvrenDev.Shared.Multitenancy;

namespace EvrenDev.Infrastructure.Identity;

internal partial class UserService
{
    public async Task<List<UserRoleDto>> GetRolesAsync(string userId, CancellationToken cancellationToken)
    {
        var userRoles = new List<UserRoleDto>();

        var user = await userManager.FindByIdAsync(userId);
        _ = user ?? throw new InternalServerException(localizer["identity.error.generic"]);

        var roles = await roleManager.Roles.AsNoTracking().ToListAsync(cancellationToken);
        foreach (var role in roles)
            userRoles.Add(new UserRoleDto
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Description = role.Description,
                Enabled = await userManager.IsInRoleAsync(user, role.Name)
            });

        return userRoles;
    }

    public async Task<string> AssignRolesAsync(string userId, UserRolesRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var user = await userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(localizer["identity.users.notfound"]);

        // Check if the user is an admin for which the admin role is getting disabled
        if (await userManager.IsInRoleAsync(user, ApiRoles.Admin)
            && request.UserRoles.Any(a => !a.Enabled && a.RoleName == ApiRoles.Admin))
        {
            // Get count of users in Admin Role
            var adminCount = (await userManager.GetUsersInRoleAsync(ApiRoles.Admin)).Count;

            // Check if user is not Root Tenant Admin
            // Edge Case : there are chances for other tenants to have users with the same email as that of Root Tenant Admin. Probably can add a check while User Registration
            if (user.Email == MultitenancyConstants.Root.EmailAddress)
            {
                if (currentTenant.Id == MultitenancyConstants.Root.Id)
                    throw new ConflictException(localizer["identity.users.roles.admin.cannotremove"]);
            }
            else if (adminCount <= 2)
            {
                throw new ConflictException(localizer["identity.users.roles.admin.minimum"]);
            }
        }

        foreach (var userRole in request.UserRoles)
            // Check if Role Exists
            if (await roleManager.FindByNameAsync(userRole.RoleName) is not null)
            {
                if (userRole.Enabled)
                {
                    if (!await userManager.IsInRoleAsync(user, userRole.RoleName))
                        await userManager.AddToRoleAsync(user, userRole.RoleName);
                }
                else
                {
                    await userManager.RemoveFromRoleAsync(user, userRole.RoleName);
                }
            }

        await events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id, true));

        return localizer["identity.users.roles.updated"];
    }
}
