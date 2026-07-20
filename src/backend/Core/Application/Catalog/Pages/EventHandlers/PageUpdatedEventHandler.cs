using EvrenDev.Application.Common.Events;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Catalog.Pages.EventHandlers;

public class PageUpdatedEventHandler(ILogger<PageUpdatedEventHandler> logger) : EventNotificationHandler<EntityUpdatedEvent<Page>>
{
    public override Task Handle(EntityUpdatedEvent<Page> @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}
