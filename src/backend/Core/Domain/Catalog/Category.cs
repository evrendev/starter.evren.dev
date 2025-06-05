namespace EvrenDev.Domain.Catalog;

public class Category(string name, string? description) : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;
    public string? Description { get; private set; } = description;
    public virtual ICollection<Course>? Courses { get; private set; }

    public Category Update(string? name, string? description)
    {
        if (name is not null && !Name.Equals(name))
            Name = name;
        if (description is not null && !Description?.Equals(description) == true)
            Description = description;
        return this;
    }
}
