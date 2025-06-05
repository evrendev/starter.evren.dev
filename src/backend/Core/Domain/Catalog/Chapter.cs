namespace EvrenDev.Domain.Catalog;

public class Chapter : AuditableEntity
{
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public Guid CourseId { get; private set; }
    public virtual Course Course { get; private set; } = default!;
    public virtual ICollection<Lesson>? Lessons { get; private set; }

    public Chapter(string title, string? description, Guid courseId)
    {
        Title = title;
        Description = description;
        CourseId = courseId;
    }

    public Chapter Update(string? title, string? description, Guid? courseId = null)
    {
        if (title is not null && !Title.Equals(title))
            Title = title;
        if (description is not null && !Description?.Equals(description) == true)
            Description = description;
        return this;
    }
}
