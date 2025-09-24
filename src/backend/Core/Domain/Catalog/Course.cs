namespace EvrenDev.Domain.Catalog;

public class Course : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = default!;
    public string? Introduction { get; private set; }
    public string? Description { get; private set; }
    public Guid CategoryId { get; private set; }
    public virtual Category Category { get; private set; } = default!;
    public string[]? Tags { get; private set; }
    public string? Image { get; private set; }
    public bool Published { get; private set; }
    public string? PreviewVideoUrl { get; private set; }
    public virtual ICollection<Chapter>? Chapters { get; private set; }
    public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = [];

    public Course(string title,
        string? introduction,
        string? description,
        Guid categoryId,
        string? image = null,
        string[]? tags = null,
        bool published = false,
        string? previewVideoUrl = null
    )
    {
        Title = title;
        Introduction = introduction;
        Description = description;
        CategoryId = categoryId;
        Image = image;
        Tags = tags;
        Published = published;
        PreviewVideoUrl = previewVideoUrl;
    }

    public Course Update(string? title, string? introduction, string? description, Guid? categoryId, string? image, string[]? tags, bool published, string? previewVideoUrl)
    {
        if (title is not null && !Title.Equals(title))
            Title = title;

        if (introduction is not null && string.Equals(Introduction, introduction) == false)
            Introduction = introduction;

        if (description is not null && string.Equals(Description, description) == false)
            Description = description;

        if (categoryId.HasValue && categoryId.Value != Guid.Empty && !CategoryId.Equals(categoryId.Value))
            CategoryId = categoryId.Value;

        if (image is not null && string.Equals(Image, image) == false)
            Image = image;

        if (tags is not null)
            Tags = tags;

        if (Published != published)
            Published = published;

        if (previewVideoUrl is not null && string.Equals(PreviewVideoUrl, previewVideoUrl) == false)
            PreviewVideoUrl = previewVideoUrl;

        return this;
    }

    public Course ClearImagePath()
    {
        Image = string.Empty;
        return this;
    }
}
