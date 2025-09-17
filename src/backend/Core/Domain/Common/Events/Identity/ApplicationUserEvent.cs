namespace EvrenDev.Domain.Common.Events.Identity;

public abstract class ApplicationUserEvent : DomainEvent
{
    protected ApplicationUserEvent(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; set; } = default!;
}
