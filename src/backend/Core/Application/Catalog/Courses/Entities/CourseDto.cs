namespace EvrenDev.Application.Catalog.Courses.Entities;

public class CourseDto : IDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public Guid CategoryId { get; set; }
    public bool Published { get; set; }
    public string? CategoryTitle { get; set; }
}
