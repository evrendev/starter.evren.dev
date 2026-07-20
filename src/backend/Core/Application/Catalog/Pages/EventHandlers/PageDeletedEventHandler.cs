using EvrenDev.Application.Common.Events;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Catalog.Pages.EventHandlers;

public class PageDeletedEventHandler(ILogger<PageDeletedEventHandler> logger) : EventNotificationHandler<EntityDeletedEvent<Page>>
{
    public override Task Handle(EntityDeletedEvent<Page> @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}
