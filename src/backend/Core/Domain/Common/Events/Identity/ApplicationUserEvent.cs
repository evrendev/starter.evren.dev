namespace EvrenDev.Domain.Common.Events.Identity;

public abstract class ApplicationUserEvent : DomainEvent
{
    public string UserId { get; set; } = default!;

    protected ApplicationUserEvent(string userId) => UserId = userId;
}
