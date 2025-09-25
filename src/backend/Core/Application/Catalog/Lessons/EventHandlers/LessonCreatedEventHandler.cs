using EvrenDev.Application.Common.Events;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Catalog.Lessons.EventHandlers;

public class LessonCreatedEventHandler(ILogger<LessonCreatedEventHandler> logger) : EventNotificationHandler<EntityCreatedEvent<Lesson>>
{
    public override Task Handle(EntityCreatedEvent<Lesson> @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}
