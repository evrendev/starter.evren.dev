namespace EvrenDev.Application.Catalog.LessonPages.Entities;

public class LessonPageExportDto : IDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string LessonTitle { get; set; } = default!;
    public int Order { get; set; }
}
