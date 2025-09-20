using EvrenDev.Application.Catalog.Categories.Entities;

namespace EvrenDev.Application.Catalog.Courses.Entities;

public class CourseDetailsDto : CourseDto
{
    public string? Introduction { get; set; }
    public string[]? Tags { get; set; }
    public string? PreviewVideoUrl { get; set; }
    public CategoryDto Category { get; set; } = default!;
}
