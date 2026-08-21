using System.ComponentModel.DataAnnotations.Schema;
using EvrenDev.Domain.Identity;

namespace EvrenDev.Domain.Catalog;

public class PageProgress : IAggregateRoot, ISoftDelete
{
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
    public Guid PageId { get; set; }
    public Page Page { get; set; } = default!;
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime LastVisitedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }

    [NotMapped]
    public List<DomainEvent> DomainEvents { get; } = [];
}
