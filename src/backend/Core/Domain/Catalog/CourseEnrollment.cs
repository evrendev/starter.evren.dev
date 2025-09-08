using EvrenDev.Domain.Identity;

namespace EvrenDev.Domain.Catalog;

public class CourseEnrollment
{
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = default!;

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public bool Completed { get; set; } = false;
}
