namespace EvrenDev.Application.Catalog.CourseEnrollments.Entities;

public class CourseEnrollmentDto : IDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = default!;
    public string? CategoryTitle { get; set; }
    public DateTime EnrolledAt { get; set; }
    public decimal PricePaid { get; set; }
    public int PercentComplete { get; set; }
    public int ChapterCount { get; set; }
    public Guid? NextChapterId { get; set; }
    public string? NextChapterTitle { get; set; }
}
