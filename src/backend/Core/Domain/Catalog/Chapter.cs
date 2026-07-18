namespace EvrenDev.Domain.Catalog;

public class Chapter : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public int Order { get; private set; }
    public Guid CourseId { get; private set; }
    // Marks a chapter created by an in-progress import (e.g. PPTX) as not yet
    // reviewed/published by an author; manual chapter creation leaves this false
    public bool IsStaging { get; private set; }
    public virtual Course Course { get; } = default!;
    public virtual ICollection<Lesson>? Lessons { get; private set; }

    public Chapter(string title, string? description, int order, Guid courseId, bool isStaging = false)
    {
        Title = title;
        Description = description;
        Order = order;
        CourseId = courseId;
        IsStaging = isStaging;
    }

    public Chapter Update(string? title, string? description, int? order, Guid? courseId = null,
        bool? isStaging = null)
    {
        if (title is not null && !Title.Equals(title))
            Title = title;

        if (description is not null && !string.Equals(Description, description))
            Description = description;

        if (order.HasValue && Order != order.Value)
            Order = order.Value;

        if (courseId.HasValue && courseId.Value != Guid.Empty && !CourseId.Equals(courseId.Value))
            CourseId = courseId.Value;

        if (isStaging.HasValue && IsStaging != isStaging.Value)
            IsStaging = isStaging.Value;

        return this;
    }
}
