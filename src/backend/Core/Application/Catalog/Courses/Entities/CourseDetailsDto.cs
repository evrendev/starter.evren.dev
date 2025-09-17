using EvrenDev.Application.Catalog.Categories.Entities;

namespace EvrenDev.Application.Catalog.Courses.Entities;

public class CourseDetailsDto : IDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Intrudiction { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public string[]? Tags { get; set; }
    public string? Image { get; set; }
    public bool Published { get; set; }
    public bool Upcoming { get; set; }
    public bool Featured { get; set; }
    public string? PreviewVideoUrl { get; set; }
    public bool Paid { get; set; }
    public bool CompletetionCertificate { get; set; }
    public bool PaidCertificate { get; set; }
    public CategoryDto Category { get; set; } = default!;
}
