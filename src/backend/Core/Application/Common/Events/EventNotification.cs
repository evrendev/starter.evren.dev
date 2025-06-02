﻿namespace EvrenDev.Application.Common.Events;

public class EventNotification<TEvent>(TEvent @event) : INotification
    where TEvent : IEvent
{
    public TEvent Event { get; } = @event;
}
