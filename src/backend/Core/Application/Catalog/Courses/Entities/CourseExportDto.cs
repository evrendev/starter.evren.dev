namespace EvrenDev.Application.Catalog.Courses.Entities;

public class CourseExportDto : IDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string CategoryName { get; set; } = default!;
}
