using EvrenDev.Application.Common.Events;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Models;
using EvrenDev.Application.Identity.Roles.Commands.Create;
using EvrenDev.Application.Identity.Roles.Commands.Update;
using EvrenDev.Application.Identity.Roles.Entities;
using EvrenDev.Application.Identity.Roles.Interfaces;
using EvrenDev.Application.Identity.Roles.Queries.Paginate;
using EvrenDev.Domain.Common.Events.Identity;
using EvrenDev.Domain.Identity;
using EvrenDev.Shared.Authorization;
using EvrenDev.Shared.Multitenancy;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace EvrenDev.Infrastructure.Identity;

internal class RoleService(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext db,
        IStringLocalizer<RoleService> localizer,
        ICurrentUser currentUser,
        ITenantInfo currentTenant,
        IEventPublisher events)
    : IRoleService
{
    public async Task<List<RoleDto>> GetListAsync(CancellationToken cancellationToken)
    {
        return (await roleManager.Roles.ToListAsync(cancellationToken))
            .Adapt<List<RoleDto>>();
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken)
    {
        return await roleManager.Roles.CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string roleName, string? excludeId)
    {
        return await roleManager.FindByNameAsync(roleName)
                   is ApplicationRole existingRole
               && existingRole.Id != excludeId;
    }

    public async Task<RoleDto> GetByIdAsync(string id)
    {
        return await db.Roles.SingleOrDefaultAsync(x => x.Id == id) is { } role
            ? role.Adapt<RoleDto>()
            : throw new NotFoundException(localizer["identity.roles.notfound"]);
    }

    public async Task<RoleDto> GetByIdWithPermissionsAsync(string roleId, CancellationToken cancellationToken)
    {
        var role = await GetByIdAsync(roleId);

        role.Permissions = await db.RoleClaims
            .Where(c => c.RoleId == roleId && c.ClaimType == ApiClaims.Permission)
            .Select(c => c.ClaimValue)
            .ToListAsync(cancellationToken);

        return role;
    }

    public async Task<string> CreateOrUpdateAsync(CreateOrUpdateRoleRequest request)
    {
        if (string.IsNullOrEmpty(request.Id))
        {
            // Create a new role.
            var role = new ApplicationRole(request.Name, request.Description);
            var result = await roleManager.CreateAsync(role);

            if (!result.Succeeded)
                throw new InternalServerException(localizer["identity.roles.create.failed"],
                    result.GetErrors(localizer));

            await events.PublishAsync(new ApplicationRoleCreatedEvent(role.Id, role.Name));

            return string.Format(localizer["identity.roles.create.success"], request.Name);
        }
        else
        {
            // Update an existing role.
            var role = await roleManager.FindByIdAsync(request.Id);

            _ = role ?? throw new NotFoundException(localizer["identity.roles.notfound"]);

            if (ApiRoles.IsDefault(role.Name))
                throw new ConflictException(string.Format(localizer["identity.roles.modify.notallowed"], role.Name));

            role.Name = request.Name;
            role.NormalizedName = request.Name.ToUpperInvariant();
            role.Description = request.Description;
            var result = await roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                throw new InternalServerException(localizer["identity.roles.update.failed"],
                    result.GetErrors(localizer));

            await events.PublishAsync(new ApplicationRoleUpdatedEvent(role.Id, role.Name));

            return string.Format(localizer["identity.roles.update.success"], role.Name);
        }
    }

    public async Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest request,
        CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.RoleId);
        _ = role ?? throw new NotFoundException(localizer["identity.roles.notfound"]);
        if (role.Name == ApiRoles.Admin)
            throw new ConflictException(localizer["identity.roles.permissions.modify.notallowed"]);

        if (currentTenant.Id != MultitenancyConstants.Root.Id)
            // Remove Root Permissions if the Role is not created for Root Tenant.
            request.Permissions.RemoveAll(u => u.StartsWith("Permissions.Root."));

        var currentClaims = await roleManager.GetClaimsAsync(role);

        // Remove permissions that were previously selected
        foreach (var claim in currentClaims.Where(c => !request.Permissions.Any(p => p == c.Value)))
        {
            var removeResult = await roleManager.RemoveClaimAsync(role, claim);
            if (!removeResult.Succeeded)
                throw new InternalServerException(localizer["identity.roles.update.permissions.failed"],
                    removeResult.GetErrors(localizer));
        }

        // Add all permissions that were not previously selected
        foreach (var permission in request.Permissions.Where(c => !currentClaims.Any(p => p.Value == c)))
            if (!string.IsNullOrEmpty(permission))
            {
                db.RoleClaims.Add(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = ApiClaims.Permission,
                    ClaimValue = permission,
                    CreatedBy = currentUser.GetUserId().ToString()
                });
                await db.SaveChangesAsync(cancellationToken);
            }

        await events.PublishAsync(new ApplicationRoleUpdatedEvent(role.Id, role.Name, true));

        return localizer["identity.roles.update.permissions.success"];
    }

    public async Task<string> DeleteAsync(string id)
    {
        var role = await roleManager.FindByIdAsync(id);

        _ = role ?? throw new NotFoundException(localizer["identity.roles.notfound"]);

        if (ApiRoles.IsDefault(role.Name))
            throw new ConflictException(string.Format(localizer["identity.roles.delete.notallowed"], role.Name));

        if ((await userManager.GetUsersInRoleAsync(role.Name)).Count > 0)
            throw new ConflictException(string.Format(localizer["identity.roles.delete.inuse"], role.Name));

        await roleManager.DeleteAsync(role);

        await events.PublishAsync(new ApplicationRoleDeletedEvent(role.Id, role.Name));

        return string.Format(localizer["identity.roles.delete.success"], role.Name);
    }

    public async Task<PaginationResponse<RoleDto>> PaginatedListAsync(PaginateRolesFilter filter,
        CancellationToken cancellationToken)
    {
        IQueryable<ApplicationRole> query = roleManager.Roles;

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var searchLower = filter.Search.ToLower();
            query = query.Where(role =>
                role.Id.ToLower().Contains(searchLower) ||
                (role.Name != null && role.Name.ToLower().Contains(searchLower)) ||
                (role.Description != null && role.Description.ToLower().Contains(searchLower))
            );
        }

        if (filter.SortBy is { Count: > 0 })
        {
            var sortItem = filter.SortBy[0];

            var isDescending = sortItem.Order?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;
            var sortField = sortItem.Key ?? string.Empty;

            switch (sortField.ToLower())
            {
                case "id":
                    query = isDescending ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id);
                    break;
                case "name":
                    query = isDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name);
                    break;
                default:
                    // Default sort order if the key is unrecognized
                    query = query.OrderBy(t => t.Id);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(t => t.Id);
        }

        var totalItems = await query.CountAsync();

        var pagedData = await query
            .Skip((filter.Page - 1) * filter.ItemsPerPage)
            .Take(filter.ItemsPerPage)
            .ToListAsync(cancellationToken);

        var pagedDataDto = pagedData.Adapt<List<RoleDto>>();

        return new PaginationResponse<RoleDto>(pagedDataDto, totalItems, filter.Page, filter.ItemsPerPage);
    }
}
