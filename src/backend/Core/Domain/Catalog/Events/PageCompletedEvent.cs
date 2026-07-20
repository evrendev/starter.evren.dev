namespace EvrenDev.Domain.Catalog.Events;

public class PageCompletedEvent(Guid pageId, string userId) : DomainEvent
{
    public Guid PageId { get; set; } = pageId;
    public string UserId { get; set; } = userId;
}
