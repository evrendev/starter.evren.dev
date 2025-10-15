using EvrenDev.Application.Catalog.Chapters.Entities;

namespace EvrenDev.Application.Catalog.Lessons.Entities;

public class LessonDetailsDto : LessonDto
{
    public string? Content { get; set; }
    public ChapterDto Chapter { get; set; } = default!;
}
