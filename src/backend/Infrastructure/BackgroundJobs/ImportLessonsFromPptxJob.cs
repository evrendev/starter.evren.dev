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
    IRepository<Chapter> chapterRepository,
    IRepository<Lesson> lessonRepository,
    IRepository<LessonPage> lessonPageRepository,
    IFileStorageService fileStorageService,
    ILogger<ImportLessonsFromPptxJob> logger) : IImportLessonsFromPptxJob
{
    public async Task ExecuteAsync(Guid courseId, string filePath, string userId, CancellationToken cancellationToken)
    {
        try
        {
            var stagingChapter = await GetOrCreateStagingChapterAsync(courseId, cancellationToken);
            var nextLessonOrder = await GetNextLessonOrderAsync(stagingChapter.Id, cancellationToken);

            var succeeded = 0;
            var failures = new List<string>();

            using (var document = PresentationDocument.Open(filePath, false))
            {
                foreach (var (slideNumber, slidePart) in PptxLessonExtractor.OpenSlides(document))
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
                        failures.Add($"Slide {slideNumber}: {ex.Message}");
                        logger.LogWarning(ex,
                            "Failed to import slide {SlideNumber} from {FilePath} for course {CourseId}",
                            slideNumber, filePath, courseId);
                    }
                }
            }

            logger.LogInformation(
                "Import finished for course {CourseId} (uploaded by {UserId}): {Succeeded} slide(s) succeeded, {Failed} failed into staging chapter {ChapterId}. Failures: {Failures}",
                courseId, userId, succeeded, failures.Count, stagingChapter.Id,
                failures.Count == 0 ? "none" : string.Join(" | ", failures));
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
