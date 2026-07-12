using EvrenDev.Application.Common.Events;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Catalog.Notes.EventHandlers;

public class NoteCreatedEventHandler(ILogger<NoteCreatedEventHandler> logger) : EventNotificationHandler<EntityCreatedEvent<Note>>
{
    public override Task Handle(EntityCreatedEvent<Note> @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}
