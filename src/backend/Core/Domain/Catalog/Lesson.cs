namespace EvrenDev.Domain.Catalog;

public class Lesson : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = default!;
    public string? Content { get; private set; }
    public Guid ChapterId { get; private set; }
    public virtual Chapter Chapter { get; } = default!;
    public virtual ICollection<Note> Notes { get; private set; } = [];
    public virtual ICollection<LessonProgress> Progress { get; private set; } = [];
    public Lesson(string title, string? content, Guid chapterId)
    {
        Title = title;
        Content = content;
        ChapterId = chapterId;
    }

    public Lesson Update(string? title, string? content, Guid? chapterId)
    {
        if (title is not null && !Title.Equals(title))
            Title = title;

        if (content is not null && !string.Equals(Content, content))
            Content = content;

        if (chapterId.HasValue && chapterId.Value != Guid.Empty && !ChapterId.Equals(chapterId.Value))
            ChapterId = chapterId.Value;

        return this;
    }
}
