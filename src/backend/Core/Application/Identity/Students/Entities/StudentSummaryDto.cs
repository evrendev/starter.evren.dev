namespace EvrenDev.Application.Identity.Students.Entities;

public class StudentSummaryDto
{
    public string UserId { get; set; } = default!;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public int EnrolledCourseCount { get; set; }
    public decimal TotalPaid { get; set; }
    public double AverageCompletionPercent { get; set; }
}
