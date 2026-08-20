using Ardalis.Specification.EntityFrameworkCore;
using EvrenDev.Application.Common.Caching;
using EvrenDev.Application.Common.Events;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Mailing;
using EvrenDev.Application.Common.Models;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Application.Identity.Users.Commands.ToggleStatus;
using EvrenDev.Application.Identity.Users.Entities;
using EvrenDev.Application.Identity.Users.Interfaces;
using EvrenDev.Application.Identity.Users.Queries.Paginate;
using EvrenDev.Domain.Common.Events.Identity;
using EvrenDev.Domain.Identity;
using EvrenDev.Infrastructure.Auth;
using EvrenDev.Shared.Authorization;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using TenantInfo = EvrenDev.Domain.Multitenancy.TenantInfo;

namespace EvrenDev.Infrastructure.Identity;

// ITenantInfo is no longer directly DI-resolvable as of Finbuckle v10 (Task S3).
internal partial class UserService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext db,
        IStringLocalizer<UserService> localizer,
        IJobService jobService,
        IMailService mailService,
        IEmailTemplateService templateService,
        IEventPublisher events,
        ICacheService cache,
        ICacheKeyService cacheKeys,
        IMultiTenantContextAccessor<TenantInfo> multiTenantContextAccessor,
        IHttpContextAccessor httpContextAccessor,
        IOptions<SecuritySettings> securitySettings)
    : IUserService
{
    private readonly SecuritySettings _securitySettings = securitySettings.Value;
    private readonly string? _currentTenantId = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id;

    public async Task<PaginationResponse<BasicUserDto>> PaginatedListAsync(PaginateUsersFilter filter,
        CancellationToken cancellationToken)
    {
        var spec = new EntitiesByPaginationFilterSpec<ApplicationUser>(filter);

        // Students are managed exclusively via the Students module (Task R1) —
        // exclude them here so the two screens never manage the same accounts.
        var studentRoleId = await roleManager.Roles
            .Where(r => r.Name == ApiRoles.Student)
            .Select(r => r.Id)
            .FirstOrDefaultAsync(cancellationToken);
        var studentUserIds = db.UserRoles
            .Where(ur => ur.RoleId == studentRoleId)
            .Select(ur => ur.UserId);

        var baseQuery = userManager.Users.Where(u => !studentUserIds.Contains(u.Id));

        var users = await baseQuery
            .WithSpecification(spec)
            .ProjectToType<BasicUserDto>()
            .ToListAsync(cancellationToken);
        var count = await baseQuery
            .CountAsync(cancellationToken);

        return new PaginationResponse<BasicUserDto>(users, count, filter.Page, filter.ItemsPerPage);
    }

    public async Task<bool> ExistsWithNameAsync(string name)
    {
        EnsureValidTenant();
        return await userManager.FindByNameAsync(name) is not null;
    }

    public async Task<bool> ExistsWithEmailAsync(string email, string? exceptId = null)
    {
        EnsureValidTenant();
        return await userManager.FindByEmailAsync(email.Normalize()) is ApplicationUser user && user.Id != exceptId;
    }

    public async Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, string? exceptId = null)
    {
        EnsureValidTenant();
        return await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) is ApplicationUser user &&
               user.Id != exceptId;
    }

    public async Task<List<UserDto>> GetListAsync(CancellationToken cancellationToken)
    {
        return (await userManager.Users
                .AsNoTracking()
                .ToListAsync(cancellationToken))
            .Adapt<List<UserDto>>();
    }

    public Task<int> GetCountAsync(CancellationToken cancellationToken)
    {
        return userManager.Users.AsNoTracking().CountAsync(cancellationToken);
    }

    public async Task<UserDto> GetAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(localizer["identity.users.notfound"]);

        var roles = await userManager.GetRolesAsync(user);

        var userDto = user.Adapt<UserDto>();
        userDto.Roles = roles;

        return userDto;
    }

    public async Task ToggleStatusAsync(ToggleUserStatusRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(localizer["identity.users.notfound"]);

        var isAdmin = await userManager.IsInRoleAsync(user, ApiRoles.Admin);
        if (isAdmin)
            throw new ConflictException(localizer["identity.users.admin.notoggle"]);

        user.IsActive = request.ActivateUser;

        await userManager.UpdateAsync(user);

        await events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));
    }

    public string? GetCurrentUserId()
    {
        return httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;
    }

    public string? GetCurrentUserEmail()
    {
        return httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;
    }

    private void EnsureValidTenant()
    {
        if (string.IsNullOrWhiteSpace(_currentTenantId))
            throw new UnauthorizedException(localizer["multitenancy.tenant.invalid"]);
    }
}
