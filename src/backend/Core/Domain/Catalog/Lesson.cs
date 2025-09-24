namespace EvrenDev.Domain.Catalog;

public class Lesson : AuditableEntity
{
    public Lesson(string title, string? description, string? content, string? notes, Guid chapterId, string? image = null)
    {
        Title = title;
        Description = description;
        Content = content;
        Notes = notes;
        ChapterId = chapterId;
        Image = image;
    }

    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? Content { get; private set; }
    public string? Notes { get; private set; }
    public string? Image { get; private set; }
    public Guid ChapterId { get; }
    public virtual Chapter Chapter { get; } = default!;
    public virtual ICollection<LessonProgress> Progress { get; set; } = [];

    public Lesson Update(string? title, string? description, string? content, string? notes, string? image)
    {
        if (title is not null && !Title.Equals(title))
            Title = title;

        if (description is not null && !Description?.Equals(description) == true)
            Description = description;

        if (content is not null && !Content?.Equals(content) == true)
            Content = content;

        if (notes is not null && !Notes?.Equals(notes) == true)
            Notes = notes;

        if (image is not null && !Image?.Equals(image) == true)
            Image = image;
        return this;
    }
}
