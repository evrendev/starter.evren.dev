using EvrenDev.Application.Common.Events;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Catalog.Notes.EventHandlers;

public class NoteDeletedEventHandler(ILogger<NoteDeletedEventHandler> logger) : EventNotificationHandler<EntityDeletedEvent<Note>>
{
    public override Task Handle(EntityDeletedEvent<Note> @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}
