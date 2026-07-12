using EvrenDev.Domain.Identity;

namespace EvrenDev.Domain.Catalog;

public class LessonPageProgress
{
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
    public Guid LessonPageId { get; set; }
    public LessonPage LessonPage { get; set; } = default!;
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime LastVisitedAt { get; set; } = DateTime.UtcNow;
}
