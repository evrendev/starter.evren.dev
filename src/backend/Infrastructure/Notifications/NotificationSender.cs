using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Shared.Notifications;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.SignalR;
using static EvrenDev.Shared.Notifications.NotificationConstants;

namespace EvrenDev.Infrastructure.Notifications;

public class NotificationSender(IHubContext<NotificationHub> notificationHubContext, ITenantInfo currentTenant)
    : INotificationSender
{
    public Task BroadcastAsync(INotificationMessage notification, CancellationToken cancellationToken)
    {
        return notificationHubContext.Clients.All
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);
    }

    public Task BroadcastAsync(INotificationMessage notification, IEnumerable<string> excludedConnectionIds,
        CancellationToken cancellationToken)
    {
        return notificationHubContext.Clients.AllExcept(excludedConnectionIds)
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);
    }

    public Task SendToAllAsync(INotificationMessage notification, CancellationToken cancellationToken)
    {
        return notificationHubContext.Clients.Group($"GroupTenant-{currentTenant.Id}")
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);
    }

    public Task SendToAllAsync(INotificationMessage notification, IEnumerable<string> excludedConnectionIds,
        CancellationToken cancellationToken)
    {
        return notificationHubContext.Clients.GroupExcept($"GroupTenant-{currentTenant.Id}", excludedConnectionIds)
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);
    }

    public Task SendToGroupAsync(INotificationMessage notification, string group, CancellationToken cancellationToken)
    {
        return notificationHubContext.Clients.Group(group)
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);
    }

    public Task SendToGroupAsync(INotificationMessage notification, string group,
        IEnumerable<string> excludedConnectionIds, CancellationToken cancellationToken)
    {
        return notificationHubContext.Clients.GroupExcept(group, excludedConnectionIds)
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);
    }

    public Task SendToGroupsAsync(INotificationMessage notification, IEnumerable<string> groupNames,
        CancellationToken cancellationToken)
    {
        return notificationHubContext.Clients.Groups(groupNames)
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);
    }

    public Task SendToUserAsync(INotificationMessage notification, string userId, CancellationToken cancellationToken)
    {
        return notificationHubContext.Clients.User(userId)
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);
    }

    public Task SendToUsersAsync(INotificationMessage notification, IEnumerable<string> userIds,
        CancellationToken cancellationToken)
    {
        return notificationHubContext.Clients.Users(userIds)
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);
    }
}
