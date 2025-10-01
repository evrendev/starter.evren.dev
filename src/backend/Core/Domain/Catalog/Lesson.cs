namespace EvrenDev.Domain.Catalog;

public class Lesson : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? Content { get; private set; }
    public string? Notes { get; private set; }
    public string? Image { get; private set; }
    public Guid ChapterId { get; private set; }
    public virtual Chapter Chapter { get; } = default!;
    public virtual ICollection<LessonProgress> Progress { get; private set; } = [];
    public Lesson(string title, string? description, string? content, string? notes, Guid chapterId, string? image = null)
    {
        Title = title;
        Description = description;
        Content = content;
        Notes = notes;
        ChapterId = chapterId;
        Image = image;
    }

    public Lesson Update(string? title, string? description, string? content, string? notes, Guid? chapterId, string? image)
    {
        if (title is not null && !Title.Equals(title))
            Title = title;

        if (description is not null && !string.Equals(Description, description))
            Description = description;

        if (content is not null && !string.Equals(Content, content))
            Content = content;

        if (notes is not null && !string.Equals(Notes, notes))
            Notes = notes;

        if (chapterId.HasValue && chapterId.Value != Guid.Empty && !ChapterId.Equals(chapterId.Value))
            ChapterId = chapterId.Value;

        if (image is not null && !string.Equals(Image, image))
            Image = image;

        return this;
    }

    public Lesson ClearImagePath()
    {
        Image = string.Empty;
        return this;
    }
}
