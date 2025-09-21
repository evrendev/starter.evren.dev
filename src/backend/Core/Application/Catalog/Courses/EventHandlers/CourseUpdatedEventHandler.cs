using EvrenDev.Application.Common.Events;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Catalog.Courses.EventHandlers;

public class CourseUpdatedEventHandler(ILogger<CourseUpdatedEventHandler> logger) : EventNotificationHandler<EntityUpdatedEvent<Course>>
{
    public override Task Handle(EntityUpdatedEvent<Course> @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}
