using EvrenDev.Application.Catalog.Lessons.Entities;

namespace EvrenDev.Application.Catalog.LessonPages.Entities;

public class LessonPageDetailsDto : LessonPageDto
{
    public string? Content { get; set; }
    public string? ContentType { get; set; }
    public string? MediaUrl { get; set; }
    public LessonDto Lesson { get; set; } = default!;
}
