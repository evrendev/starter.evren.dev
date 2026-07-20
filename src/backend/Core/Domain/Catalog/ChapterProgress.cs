using System.ComponentModel.DataAnnotations.Schema;
using EvrenDev.Domain.Identity;

namespace EvrenDev.Domain.Catalog;

public enum ProgressStatus { NotStarted = 0, InProgress = 1, Completed = 2 }

public class ChapterProgress : IAggregateRoot
{
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    public Guid ChapterId { get; set; } = default!;
    public Chapter Chapter { get; set; } = default!;

    public ProgressStatus Status { get; set; } = ProgressStatus.NotStarted;
    public int PercentComplete { get; set; } = 0;
    public Guid? LastVisitedPageId { get; set; }
    public DateTime? CompletedAt { get; set; }

    [NotMapped]
    public List<DomainEvent> DomainEvents { get; } = [];
}
