namespace EvrenDev.Domain.Catalog.Events;

public class ChapterCompletedEvent(Guid chapterId, string userId) : DomainEvent
{
    public Guid ChapterId { get; set; } = chapterId;
    public string UserId { get; set; } = userId;
}
