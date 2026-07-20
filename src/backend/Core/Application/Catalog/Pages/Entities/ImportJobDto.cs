using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Entities;

public class ImportJobDto : IDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid? ChapterId { get; set; }
    public ImportStatus Status { get; set; }
    public int TotalSlides { get; set; }
    public int ProcessedSlides { get; set; }
    public int SucceededSlides { get; set; }
    public int FailedSlides { get; set; }
    public string? ErrorsJson { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int PercentComplete => TotalSlides <= 0
        ? 0
        : (int)Math.Round(ProcessedSlides * 100.0 / TotalSlides);
}
