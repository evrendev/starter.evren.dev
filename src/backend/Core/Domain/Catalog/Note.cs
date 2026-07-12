namespace EvrenDev.Domain.Catalog;

public class Note : AuditableEntity, IAggregateRoot
{
    public string UserId { get; private set; } = default!;
    public Guid LessonPageId { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public virtual LessonPage? LessonPage { get; } = default!;

    public Note(string userId, Guid lessonPageId, string content)
    {
        UserId = userId;
        LessonPageId = lessonPageId;
        Content = content;
    }

    public Note Update(string? userId, Guid lessonPageId, string? content)
    {
        if (userId is not null && !UserId.Equals(userId))
            UserId = userId;

        if (lessonPageId != Guid.Empty && LessonPageId != lessonPageId)
            LessonPageId = lessonPageId;

        if (content is not null && !Content.Equals(content))
            Content = content;

        return this;
    }
}
