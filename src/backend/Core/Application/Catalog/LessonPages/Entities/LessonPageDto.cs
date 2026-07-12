namespace EvrenDev.Application.Catalog.LessonPages.Entities;

public class LessonPageDto : IDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public Guid LessonId { get; set; }
    public string? LessonTitle { get; set; }
    public int Order { get; set; }
}
