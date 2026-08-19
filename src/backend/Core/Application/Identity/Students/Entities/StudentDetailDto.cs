namespace EvrenDev.Application.Identity.Students.Entities;

// Backing data for the Students detail screen (Task R2) — added alongside the
// frontend since StudentSummaryDto (Task R1) only carries aggregate counts,
// not the per-course/per-payment breakdown the detail page needs to render.
public class StudentDetailDto
{
    public string UserId { get; set; } = default!;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public List<StudentEnrollmentDto> Enrollments { get; set; } = [];
    public List<StudentPaymentDto> Payments { get; set; } = [];
}

public class StudentEnrollmentDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = default!;
    public DateTime EnrolledAt { get; set; }
    public decimal PricePaid { get; set; }
    public int PercentComplete { get; set; }
}

public class StudentPaymentDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? PayPalCaptureId { get; set; }
    public DateTime? CapturedAt { get; set; }
}
