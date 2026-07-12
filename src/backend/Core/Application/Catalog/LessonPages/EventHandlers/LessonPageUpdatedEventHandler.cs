using EvrenDev.Application.Common.Events;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Catalog.LessonPages.EventHandlers;

public class LessonPageUpdatedEventHandler(ILogger<LessonPageUpdatedEventHandler> logger) : EventNotificationHandler<EntityUpdatedEvent<LessonPage>>
{
    public override Task Handle(EntityUpdatedEvent<LessonPage> @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}
