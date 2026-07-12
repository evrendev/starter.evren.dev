namespace EvrenDev.Domain.Catalog.Events;

public class LessonCompletedEvent(Guid lessonId, string userId) : DomainEvent
{
    public Guid LessonId { get; set; } = lessonId;
    public string UserId { get; set; } = userId;
}
