namespace EvrenDev.Application.Catalog.Lessons.Entities;

public class LessonDto : IDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public Guid ChapterId { get; set; }
    public string? ChapterTitle { get; set; }
}
