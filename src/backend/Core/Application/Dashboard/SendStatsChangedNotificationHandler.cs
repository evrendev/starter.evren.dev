using EvrenDev.Application.Common.Events;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;
using EvrenDev.Domain.Common.Events.Identity;
using EvrenDev.Shared.Notifications;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Dashboard;

public class SendStatsChangedNotificationHandler(ILogger<SendStatsChangedNotificationHandler> logger, INotificationSender notifications)
    :
        IEventNotificationHandler<EntityCreatedEvent<Brand>>,
        IEventNotificationHandler<EntityDeletedEvent<Brand>>,
        IEventNotificationHandler<EntityCreatedEvent<Product>>,
        IEventNotificationHandler<EntityDeletedEvent<Product>>,
        IEventNotificationHandler<ApplicationRoleCreatedEvent>,
        IEventNotificationHandler<ApplicationRoleDeletedEvent>,
        IEventNotificationHandler<ApplicationUserCreatedEvent>
{
    public Task Handle(EventNotification<EntityCreatedEvent<Brand>> notification, CancellationToken cancellationToken) =>
        SendStatsChangedNotification(notification.Event, cancellationToken);
    public Task Handle(EventNotification<EntityDeletedEvent<Brand>> notification, CancellationToken cancellationToken) =>
        SendStatsChangedNotification(notification.Event, cancellationToken);
    public Task Handle(EventNotification<EntityCreatedEvent<Product>> notification, CancellationToken cancellationToken) =>
        SendStatsChangedNotification(notification.Event, cancellationToken);
    public Task Handle(EventNotification<EntityDeletedEvent<Product>> notification, CancellationToken cancellationToken) =>
        SendStatsChangedNotification(notification.Event, cancellationToken);
    public Task Handle(EventNotification<ApplicationRoleCreatedEvent> notification, CancellationToken cancellationToken) =>
        SendStatsChangedNotification(notification.Event, cancellationToken);
    public Task Handle(EventNotification<ApplicationRoleDeletedEvent> notification, CancellationToken cancellationToken) =>
        SendStatsChangedNotification(notification.Event, cancellationToken);
    public Task Handle(EventNotification<ApplicationUserCreatedEvent> notification, CancellationToken cancellationToken) =>
        SendStatsChangedNotification(notification.Event, cancellationToken);

    private Task SendStatsChangedNotification(IEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("{event} Triggered => Sending StatsChangedNotification", @event.GetType().Name);

        return notifications.SendToAllAsync(new StatsChangedNotification(), cancellationToken);
    }
}
