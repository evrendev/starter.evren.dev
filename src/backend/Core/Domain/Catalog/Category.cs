namespace EvrenDev.Domain.Catalog;

public class Category : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public virtual ICollection<Course>? Courses { get; private set; }

    public Category(string title, string? description)
    {
        Title = title;
        Description = description;
    }

    public Category Update(string? title, string? description)
    {
        if (title is not null && !Title.Equals(title))
            Title = title;

        if (description is not null && !string.Equals(Description, description))
            Description = description;

        return this;
    }
}
