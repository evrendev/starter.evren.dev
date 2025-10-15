namespace EvrenDev.Domain.Catalog;

public class Note : AuditableEntity, IAggregateRoot
{
    public string UserId { get; private set; } = default!;
    public Guid LessonId { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public virtual Lesson? Lesson { get; } = default!;

    public Note(string userId, Guid lessonId, string content)
    {
        UserId = userId;
        LessonId = lessonId;
        Content = content;
    }

    public Note Update(string? userId, Guid lessonId, string? content)
    {
        if (userId is not null && !UserId.Equals(userId))
            UserId = userId;

        if (lessonId != Guid.Empty && LessonId != lessonId)
            LessonId = lessonId;

        if (content is not null && !Content.Equals(content))
            Content = content;

        return this;
    }
}
