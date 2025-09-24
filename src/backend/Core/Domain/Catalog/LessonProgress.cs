using EvrenDev.Domain.Identity;

namespace EvrenDev.Domain.Catalog;

public class LessonProgress
{
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = default!;

    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    public bool Completed { get; set; } = false;
}
