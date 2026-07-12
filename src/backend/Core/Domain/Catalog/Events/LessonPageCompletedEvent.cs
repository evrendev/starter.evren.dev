namespace EvrenDev.Domain.Catalog.Events;

public class LessonPageCompletedEvent(Guid lessonPageId, string userId) : DomainEvent
{
    public Guid LessonPageId { get; set; } = lessonPageId;
    public string UserId { get; set; } = userId;
}
