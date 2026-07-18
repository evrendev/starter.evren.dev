using EvrenDev.Application.Catalog.Lessons.Interfaces;
using EvrenDev.Application.Common.FileStorage;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.BackgroundJobs;

// Scaffold only: proves the upload -> enqueue -> execute pipeline end-to-end.
// Real .pptx parsing (DocumentFormat.OpenXml) is a separate follow-up task.
public class ImportLessonsFromPptxJob(
    IFileStorageService fileStorageService,
    ILogger<ImportLessonsFromPptxJob> logger) : IImportLessonsFromPptxJob
{
    public Task ExecuteAsync(Guid courseId, string filePath, string userId, CancellationToken cancellationToken)
    {
        var sizeBytes = File.Exists(filePath) ? new FileInfo(filePath).Length : 0;

        logger.LogInformation(
            "Received pptx for course {CourseId}, {Bytes} bytes (uploaded by {UserId}, temp file {FilePath}). Parsing not yet implemented.",
            courseId, sizeBytes, userId, filePath);

        fileStorageService.Remove(filePath);

        return Task.CompletedTask;
    }
}
