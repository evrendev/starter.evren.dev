namespace EvrenDev.Application.Catalog.Lessons.Entities;

public class LessonExportDto : IDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string ChapterTitle { get; set; } = default!;
}
