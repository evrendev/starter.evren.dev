using EvrenDev.Application.Common.Events;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Catalog.LessonPages.EventHandlers;

public class LessonPageDeletedEventHandler(ILogger<LessonPageDeletedEventHandler> logger) : EventNotificationHandler<EntityDeletedEvent<LessonPage>>
{
    public override Task Handle(EntityDeletedEvent<LessonPage> @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}
