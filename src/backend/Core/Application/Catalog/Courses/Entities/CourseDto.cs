namespace EvrenDev.Application.Catalog.Courses.Entities;

public class CourseDto : IDto
{
    public Guid Id { get; set; }
    public string Title { get; private set; } = default!;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
}
