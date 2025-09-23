using EvrenDev.Application.Common.Events;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Catalog.Chapters.EventHandlers;

public class ChapterUpdatedEventHandler(ILogger<ChapterUpdatedEventHandler> logger) : EventNotificationHandler<EntityUpdatedEvent<Chapter>>
{
    public override Task Handle(EntityUpdatedEvent<Chapter> @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}
