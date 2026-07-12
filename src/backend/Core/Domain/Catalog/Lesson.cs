namespace EvrenDev.Domain.Catalog;

public class Lesson : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = default!;
    public int Order { get; private set; }
    public Guid ChapterId { get; private set; }
    public virtual Chapter Chapter { get; } = default!;
    public virtual ICollection<LessonPage> Pages { get; private set; } = [];
    public virtual ICollection<LessonProgress> Progress { get; private set; } = [];

    public Lesson(string title, int order, Guid chapterId)
    {
        Title = title;
        Order = order;
        ChapterId = chapterId;
    }

    public Lesson Update(string? title, int? order, Guid? chapterId)
    {
        if (title is not null && !Title.Equals(title))
            Title = title;

        if (order.HasValue && Order != order.Value)
            Order = order.Value;

        if (chapterId.HasValue && chapterId.Value != Guid.Empty && !ChapterId.Equals(chapterId.Value))
            ChapterId = chapterId.Value;

        return this;
    }
}
