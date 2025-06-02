using EvrenDev.Application.Common.Events;
using EvrenDev.Shared.Events;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.Common.Services;

public class EventPublisher(IPublisher mediator) : IEventPublisher
{
    public Task PublishAsync(IEvent @event)
    {
        var eventName = @event?.GetType().Name;
        LoggerMessage.Define<EventPublisher>(LogLevel.Information, new EventId(1, eventName), $"Publishing Event : {eventName}");
        return mediator.Publish(CreateEventNotification(@event!));
    }

    private static INotification CreateEventNotification(IEvent @event) =>
        (INotification)Activator.CreateInstance(
            typeof(EventNotification<>).MakeGenericType(@event.GetType()), @event)!;
}
