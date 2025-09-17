namespace EvrenDev.Domain.Catalog;

public class Chapter : AuditableEntity
{
    public Chapter(string title, string? description, Guid courseId)
    {
        Title = title;
        Description = description;
        CourseId = courseId;
    }

    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public Guid CourseId { get; }
    public virtual Course Course { get; } = default!;
    public virtual ICollection<Lesson>? Lessons { get; private set; }

    public Chapter Update(string? title, string? description, Guid? courseId = null)
    {
        if (title is not null && !Title.Equals(title))
            Title = title;
        if (description is not null && !Description?.Equals(description) == true)
            Description = description;
        return this;
    }
}
