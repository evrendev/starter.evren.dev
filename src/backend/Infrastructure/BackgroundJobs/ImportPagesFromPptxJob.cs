using System.Text.Json;
using DocumentFormat.OpenXml.Packaging;
using EvrenDev.Application.Catalog.Chapters.Specifications;
using EvrenDev.Application.Catalog.Pages.Interfaces;
using EvrenDev.Application.Catalog.Pages.Specifications;
using EvrenDev.Application.Common.FileStorage;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Enums;
using EvrenDev.Infrastructure.Import;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.BackgroundJobs;

public class ImportPagesFromPptxJob(
    IRepository<ImportJob> importJobRepository,
    IRepository<Chapter> chapterRepository,
    IRepository<Page> pageRepository,
    IFileStorageService fileStorageService,
    ILogger<ImportPagesFromPptxJob> logger) : IImportPagesFromPptxJob
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
        List<string>? slidesHtml, CancellationToken cancellationToken)
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
                var nextPageOrder = await GetNextPageOrderAsync(stagingChapter.Id, cancellationToken);

                importJob.MarkProcessing(stagingChapter.Id, slides.Count);
                await importJobRepository.UpdateAsync(importJob, cancellationToken);

                var processed = 0;
                foreach (var (slideNumber, slidePart) in slides)
                {
                    try
                    {
                        await ImportSlideAsync(slidePart, slideNumber, stagingChapter.Id, nextPageOrder,
                            slidesHtml, cancellationToken);
                        nextPageOrder++;
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

    private async Task<int> GetNextPageOrderAsync(Guid stagingChapterId, CancellationToken cancellationToken)
    {
        var existingPages = await pageRepository.ListAsync(new PagesByChapterSpec(stagingChapterId),
            cancellationToken);

        return existingPages.Count == 0 ? 0 : existingPages.Max(p => p.Order) + 1;
    }

    private async Task ImportSlideAsync(SlidePart slidePart, int slideNumber, Guid stagingChapterId, int pageOrder,
        List<string>? slidesHtml, CancellationToken cancellationToken)
    {
        var extracted = PptxLessonExtractor.ExtractSlide(slidePart, slideNumber);

        var mediaUrl = extracted.ContentType == PageContentType.Embed
            ? extracted.EmbedUrl
            : extracted.Media is null
                ? null
                : await UploadMediaAsync(extracted, cancellationToken);

        // Title/ContentType/MediaUrl always come from the backend's own OpenXml pass
        // (unchanged); Content prefers the richer client-side pptx-to-html render for
        // this slide index when present, falling back to the plain-text extraction
        // otherwise — e.g. client parsing failed, or the payload wasn't sent at all
        var clientHtml = slideNumber - 1 < slidesHtml?.Count ? slidesHtml![slideNumber - 1] : null;
        var isImported = !string.IsNullOrWhiteSpace(clientHtml);
        var contentHtml = isImported ? clientHtml! : extracted.ContentHtml;

        // All imported pages start out needing a human review pass, regardless of the
        // detected ContentType (spec: applies uniformly, not just to the ambiguous cases)
        var page = new Page(extracted.Title, contentHtml, extracted.ContentType,
            pageOrder, stagingChapterId, mediaUrl, needsReview: true, isImported: isImported);
        await pageRepository.AddAsync(page, cancellationToken);
    }

    private async Task<string> UploadMediaAsync(ExtractedSlide extracted, CancellationToken cancellationToken)
    {
        var media = extracted.Media!;
        var fileType = extracted.ContentType == PageContentType.Video ? FileType.Video : FileType.Image;

        var uploadRequest = new FileUploadRequest
        {
            Name = $"{Guid.NewGuid()}{media.Extension}",
            Extension = media.Extension,
            Data = Convert.ToBase64String(media.Bytes),
        };

        return await fileStorageService.UploadAsync<Page>(uploadRequest, fileType, cancellationToken);
    }
}
