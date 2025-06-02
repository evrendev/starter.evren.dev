using EvrenDev.Application.Common.Events;
using EvrenDev.Application.Identity.Users;
using EvrenDev.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Infrastructure.Identity;

internal class InvalidateUserPermissionCacheHandler(IUserService userService, UserManager<ApplicationUser> userManager) :
    IEventNotificationHandler<ApplicationUserUpdatedEvent>,
    IEventNotificationHandler<ApplicationRoleUpdatedEvent>
{
    public async Task Handle(EventNotification<ApplicationUserUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        if (notification.Event.RolesUpdated)
        {
            await userService.InvalidatePermissionCacheAsync(notification.Event.UserId, cancellationToken);
        }
    }

    public async Task Handle(EventNotification<ApplicationRoleUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        if (notification.Event.PermissionsUpdated)
        {
            foreach (var user in await userManager.GetUsersInRoleAsync(notification.Event.RoleName))
            {
                await userService.InvalidatePermissionCacheAsync(user.Id, cancellationToken);
            }
        }
    }
}
