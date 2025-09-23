namespace EvrenDev.Domain.Catalog;

public class Category(string title, string? description) : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = title;
    public string? Description { get; private set; } = description;
    public virtual ICollection<Course>? Courses { get; private set; }

    public Category Update(string? title, string? description)
    {
        if (title is not null && !Title.Equals(title))
            Title = title;
        if (description is not null && !Description?.Equals(description) == true)
            Description = description;
        return this;
    }
}
