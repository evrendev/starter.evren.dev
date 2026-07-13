namespace EvrenDev.Application.Catalog.CourseEnrollments.Entities;

public class CourseEnrollmentDto : IDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = default!;
    public DateTime EnrolledAt { get; set; }
    public decimal PricePaid { get; set; }
    public int PercentComplete { get; set; }
}
