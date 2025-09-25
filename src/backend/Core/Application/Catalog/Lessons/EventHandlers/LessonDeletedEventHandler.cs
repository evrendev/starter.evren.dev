using EvrenDev.Application.Common.Events;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Catalog.Lessons.EventHandlers;

public class LessonDeletedEventHandler(ILogger<LessonDeletedEventHandler> logger) : EventNotificationHandler<EntityDeletedEvent<Lesson>>
{
    public override Task Handle(EntityDeletedEvent<Lesson> @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}
