using EvrenDev.Application.Common.Events;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Catalog.Chapters.EventHandlers;

public class ChapterDeletedEventHandler(ILogger<ChapterDeletedEventHandler> logger) : EventNotificationHandler<EntityDeletedEvent<Chapter>>
{
    public override Task Handle(EntityDeletedEvent<Chapter> @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}
