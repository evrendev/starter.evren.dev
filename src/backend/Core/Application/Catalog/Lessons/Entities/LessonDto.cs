namespace EvrenDev.Application.Catalog.Lessons.Entities;

public class LessonDto : IDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public Guid ChapterId { get; set; }
    public string? ChapterTitle { get; set; }
    // Both computed via Mapster custom mapping (Infrastructure/Mapping/MapsterSettings.cs) —
    // not direct Lesson members, so Mapster's default flattening convention can't reach them.
    public bool IsStaging { get; set; }
    public bool NeedsReview { get; set; }
}
