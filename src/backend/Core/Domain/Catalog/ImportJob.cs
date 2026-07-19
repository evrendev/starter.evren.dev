namespace EvrenDev.Domain.Catalog;

public enum ImportStatus
{
    Queued = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3
}

// Tracks a single PPTX-to-lessons import run so the client can poll for progress
// instead of relying on Hangfire's own dashboard/storage (see docs/lms-domain.md).
// Status=Failed means the whole run aborted (e.g. the file could not be opened at
// all); a run that completes with some bad slides is still Completed, with the
// per-slide failures reflected in FailedSlides/ErrorsJson (see ImportLessonsFromPptxJob,
// which already tolerates and skips individual bad slides).
public class ImportJob : AuditableEntity, IAggregateRoot
{
    public Guid CourseId { get; private set; }
    public Guid? ChapterId { get; private set; }
    public ImportStatus Status { get; private set; }
    public int TotalSlides { get; private set; }
    public int ProcessedSlides { get; private set; }
    public int SucceededSlides { get; private set; }
    public int FailedSlides { get; private set; }
    public string? ErrorsJson { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public ImportJob(Guid courseId, int totalSlides = 0)
    {
        CourseId = courseId;
        TotalSlides = totalSlides;
        Status = ImportStatus.Queued;
    }

    public void MarkProcessing(Guid chapterId, int totalSlides)
    {
        Status = ImportStatus.Processing;
        ChapterId = chapterId;
        TotalSlides = totalSlides;
        StartedAt = DateTime.UtcNow;
    }

    public void UpdateProgress(int processedSlides, int succeededSlides, int failedSlides)
    {
        ProcessedSlides = processedSlides;
        SucceededSlides = succeededSlides;
        FailedSlides = failedSlides;
    }

    public void MarkCompleted(int succeededSlides, int failedSlides, string? errorsJson)
    {
        Status = ImportStatus.Completed;
        ProcessedSlides = succeededSlides + failedSlides;
        SucceededSlides = succeededSlides;
        FailedSlides = failedSlides;
        ErrorsJson = errorsJson;
        CompletedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string errorMessage)
    {
        Status = ImportStatus.Failed;
        ErrorsJson = errorMessage;
        CompletedAt = DateTime.UtcNow;
    }
}
