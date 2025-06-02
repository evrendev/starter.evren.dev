using EvrenDev.Application.Common.Events;
using EvrenDev.Shared.Events;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.Common.Services;

public class EventPublisher(IPublisher mediator, ILogger<EventPublisher> logger) : IEventPublisher
{
    public Task PublishAsync(IEvent @event)
    {
        var eventName = @event?.GetType().Name;
        logger.LogInformation("Publishing Event : {EventName}", eventName);
        return mediator.Publish(CreateEventNotification(@event!));
    }

    private static INotification CreateEventNotification(IEvent @event) =>
        (INotification)Activator.CreateInstance(
            typeof(EventNotification<>).MakeGenericType(@event.GetType()), @event)!;
}
