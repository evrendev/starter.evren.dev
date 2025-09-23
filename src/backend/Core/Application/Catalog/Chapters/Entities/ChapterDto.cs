namespace EvrenDev.Application.Catalog.Chapters.Entities;

public class ChapterDto : IDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid CourseId { get; set; }
    public string? CourseTitle { get; set; }
}
