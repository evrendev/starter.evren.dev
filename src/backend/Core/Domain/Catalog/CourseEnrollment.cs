using System.ComponentModel.DataAnnotations.Schema;
using EvrenDev.Domain.Identity;

namespace EvrenDev.Domain.Catalog;

public class CourseEnrollment : IAggregateRoot, ISoftDelete
{
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = default!;

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public decimal PricePaid { get; set; }
    public int PercentComplete { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }

    [NotMapped]
    public List<DomainEvent> DomainEvents { get; } = [];
}
