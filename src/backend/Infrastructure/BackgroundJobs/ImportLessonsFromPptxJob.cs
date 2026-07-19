using System.Text.Json;
using DocumentFormat.OpenXml.Packaging;
using EvrenDev.Application.Catalog.Chapters.Specifications;
using EvrenDev.Application.Catalog.Lessons.Interfaces;
using EvrenDev.Application.Catalog.Lessons.Specifications;
using EvrenDev.Application.Common.FileStorage;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Enums;
using EvrenDev.Infrastructure.Import;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.BackgroundJobs;

public class ImportLessonsFromPptxJob(
    IRepository<ImportJob> importJobRepository,
    IRepository<Chapter> chapterRepository,
    IRepository<Lesson> lessonRepository,
    IRepository<LessonPage> lessonPageRepository,
    IFileStorageService fileStorageService,
    ILogger<ImportLessonsFromPptxJob> logger) : IImportLessonsFromPptxJob
{
    private record SlideFailure(int SlideIndex, string Message);

    // Every mutating repository call (Add/Update) triggers its own SaveChangesAsync
    // (Ardalis.Specification.EntityFrameworkCore's RepositoryBase — confirmed in Task B),
    // so a progress write is a real DB round-trip, not a cheap in-memory update. Writing
    // on every single slide would mean e.g. 500 extra round-trips for a 500-slide deck.
    // Batching every 10 slides keeps polling responsive (updates land within ~10 slides'
    // worth of processing time) while capping the extra round-trips at total/10. The
    // final, authoritative counts are always written once more via MarkCompleted/MarkFailed
    // regardless of where the last batch landed, so correctness never depends on the batch size.
    private const int ProgressBatchSize = 10;

    public async Task ExecuteAsync(Guid importJobId, Guid courseId, string filePath, string userId,
        CancellationToken cancellationToken)
    {
        var importJob = await importJobRepository.GetByIdAsync(importJobId, cancellationToken)
            ?? throw new InvalidOperationException($"ImportJob '{importJobId}' not found.");

        try
        {
            var succeeded = 0;
            var failures = new List<SlideFailure>();

            using (var document = PresentationDocument.Open(filePath, false))
            {
                var slides = PptxLessonExtractor.OpenSlides(document).ToList();

                var stagingChapter = await GetOrCreateStagingChapterAsync(courseId, cancellationToken);
                var nextLessonOrder = await GetNextLessonOrderAsync(stagingChapter.Id, cancellationToken);

                importJob.MarkProcessing(stagingChapter.Id, slides.Count);
                await importJobRepository.UpdateAsync(importJob, cancellationToken);

                var processed = 0;
                foreach (var (slideNumber, slidePart) in slides)
                {
                    try
                    {
                        await ImportSlideAsync(slidePart, slideNumber, stagingChapter.Id, nextLessonOrder,
                            cancellationToken);
                        nextLessonOrder++;
                        succeeded++;
                    }
                    catch (Exception ex)
                    {
                        failures.Add(new SlideFailure(slideNumber, ex.Message));
                        logger.LogWarning(ex,
                            "Failed to import slide {SlideNumber} from {FilePath} for course {CourseId}",
                            slideNumber, filePath, courseId);
                    }

                    processed++;
                    if (processed % ProgressBatchSize == 0)
                    {
                        importJob.UpdateProgress(processed, succeeded, failures.Count);
                        await importJobRepository.UpdateAsync(importJob, cancellationToken);
                    }
                }

                var errorsJson = failures.Count == 0 ? null : JsonSerializer.Serialize(failures);
                importJob.MarkCompleted(succeeded, failures.Count, errorsJson);
                await importJobRepository.UpdateAsync(importJob, cancellationToken);

                logger.LogInformation(
                    "Import finished for course {CourseId} (uploaded by {UserId}): {Succeeded} slide(s) succeeded, {Failed} failed into staging chapter {ChapterId}. Failures: {Failures}",
                    courseId, userId, succeeded, failures.Count, stagingChapter.Id,
                    failures.Count == 0 ? "none" : errorsJson);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Import job {ImportJobId} for course {CourseId} aborted", importJobId, courseId);
            importJob.MarkFailed(ex.Message);
            await importJobRepository.UpdateAsync(importJob, cancellationToken);
            throw;
        }
        finally
        {
            fileStorageService.Remove(filePath);
        }
    }

    private async Task<Chapter> GetOrCreateStagingChapterAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var existing = await chapterRepository.FirstOrDefaultAsync(new StagingChapterByCourseSpec(courseId),
            cancellationToken);
        if (existing is not null)
            return existing;

        var siblingChapters = await chapterRepository.ListAsync(new ChaptersByCourseSpec(courseId),
            cancellationToken);
        var order = siblingChapters.Count == 0 ? 0 : siblingChapters.Max(c => c.Order) + 1;

        // Fixed, admin-facing-only label — deliberately not localized (see task discussion):
        // this chapter only ever appears to authors reviewing an import, not learners
        var stagingChapter = new Chapter("Imported (Unreviewed)", null, order, courseId, isStaging: true);
        await chapterRepository.AddAsync(stagingChapter, cancellationToken);

        return stagingChapter;
    }

    private async Task<int> GetNextLessonOrderAsync(Guid stagingChapterId, CancellationToken cancellationToken)
    {
        var existingLessons = await lessonRepository.ListAsync(new LessonsByChapterSpec(stagingChapterId),
            cancellationToken);

        return existingLessons.Count == 0 ? 0 : existingLessons.Max(l => l.Order) + 1;
    }

    private async Task ImportSlideAsync(SlidePart slidePart, int slideNumber, Guid stagingChapterId, int lessonOrder,
        CancellationToken cancellationToken)
    {
        var extracted = PptxLessonExtractor.ExtractSlide(slidePart, slideNumber);

        var mediaUrl = extracted.ContentType == LessonPageContentType.Embed
            ? extracted.EmbedUrl
            : extracted.Media is null
                ? null
                : await UploadMediaAsync(extracted, cancellationToken);

        var lesson = new Lesson(extracted.Title, lessonOrder, stagingChapterId);
        await lessonRepository.AddAsync(lesson, cancellationToken);

        // All imported pages start out needing a human review pass, regardless of the
        // detected ContentType (spec: applies uniformly, not just to the ambiguous cases)
        var lessonPage = new LessonPage(extracted.Title, extracted.ContentHtml, extracted.ContentType,
            0, lesson.Id, mediaUrl, needsReview: true);
        await lessonPageRepository.AddAsync(lessonPage, cancellationToken);
    }

    private async Task<string> UploadMediaAsync(ExtractedSlide extracted, CancellationToken cancellationToken)
    {
        var media = extracted.Media!;
        var fileType = extracted.ContentType == LessonPageContentType.Video ? FileType.Video : FileType.Image;

        var uploadRequest = new FileUploadRequest
        {
            Name = $"{Guid.NewGuid()}{media.Extension}",
            Extension = media.Extension,
            Data = Convert.ToBase64String(media.Bytes),
        };

        return await fileStorageService.UploadAsync<LessonPage>(uploadRequest, fileType, cancellationToken);
    }
}
