namespace EvrenDev.Domain.Catalog;

public class Course : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = default!;
    public string? Intrudiction { get; private set; }
    public string? Description { get; private set; }
    public Guid CategoryId { get; private set; }
    public virtual Category Category { get; private set; } = default!;
    public string[]? Tags { get; private set; }
    public string? Image { get; private set; }
    public bool Published { get; private set; }
    public bool Upcoming { get; private set; }
    public bool Featured { get; private set; }
    public string? PreviewVideoUrl { get; private set; }
    public bool Paid { get; private set; }
    public bool CompletetionCertificate { get; private set; }
    public bool PaidCertificate { get; private set; }
    public virtual ICollection<Chapter>? Chapters { get; private set; }
    public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();

    public Course(string title,
        string? intrudiction,
        string? description,
        Guid categoryId,
        string? image = null,
        string[]? tags = null,
        bool published = false,
        bool upcoming = false,
        bool featured = false,
        string? previewVideoUrl = null,
        bool paid = false,
        bool completetionCertificate = false,
        bool paidCertificate = false
    )
    {
        Title = title;
        Intrudiction = intrudiction;
        Description = description;
        CategoryId = categoryId;
        Tags = tags;
        Image = image;
        Published = published;
        Upcoming = upcoming;
        Featured = featured;
        PreviewVideoUrl = previewVideoUrl;
        Paid = paid;
        CompletetionCertificate = completetionCertificate;
        PaidCertificate = paidCertificate;
    }

    public Course Update(string? title, string? intrudiction, string? description, Guid? categoryId, string? image, string[]? tags, bool published, bool upcoming, bool featured, string? previewVideoUrl, bool paid, bool completetionCertificate, bool paidCertificate)
    {
        if (title is not null && !Title.Equals(title))
            Title = title;

        if (intrudiction is not null && !Intrudiction?.Equals(intrudiction) == true)
            Intrudiction = intrudiction;

        if (description is not null && !Description?.Equals(description) == true)
            Description = description;

        if (categoryId.HasValue && categoryId.Value != Guid.Empty && !CategoryId.Equals(categoryId.Value))
            CategoryId = categoryId.Value;

        if (image is not null && !Image?.Equals(image) == true)
            Image = image;

        if (tags is not null)
            Tags = tags;

        if (Published != published)
            Published = published;

        if (Upcoming != upcoming)
            Upcoming = upcoming;

        if (Featured != featured)
            Featured = featured;

        if (previewVideoUrl is not null && !PreviewVideoUrl?.Equals(previewVideoUrl) == true)
            PreviewVideoUrl = previewVideoUrl;

        if (Paid != paid)
            Paid = paid;

        if (CompletetionCertificate != completetionCertificate)
            CompletetionCertificate = completetionCertificate;

        if (PaidCertificate != paidCertificate)
            PaidCertificate = paidCertificate;

        return this;
    }

    public Course ClearImagePath()
    {
        Image = string.Empty;
        return this;
    }
}
