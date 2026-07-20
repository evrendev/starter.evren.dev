using EvrenDev.Application.Catalog.CourseEnrollments.Specifications;
using EvrenDev.Application.Catalog.Pages.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Queries.Get;

public class ChapterPlayerPageDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public int Order { get; set; }
    public string ContentType { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string? MediaUrl { get; set; }
    public bool IsImported { get; set; }
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class GetChapterPlayerRequest(Guid chapterId) : IRequest<ChapterPlayerDto>
{
    public Guid ChapterId { get; set; } = chapterId;
}

public class ChapterPlayerDto
{
    public Guid ChapterId { get; set; }
    public string ChapterTitle { get; set; } = default!;
    public List<ChapterPlayerPageDto> Pages { get; set; } = [];
    public int PercentComplete { get; set; }
    public Guid? LastVisitedPageId { get; set; }
}

public class GetChapterPlayerRequestHandler(
    IReadRepository<Chapter> chapterRepository,
    IReadRepository<PageProgress> pageProgressRepository,
    IReadRepository<ChapterProgress> chapterProgressRepository,
    IReadRepository<CourseEnrollment> courseEnrollmentRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetChapterPlayerRequest, ChapterPlayerDto>
{
    public async Task<ChapterPlayerDto> Handle(GetChapterPlayerRequest request, CancellationToken cancellationToken)
    {
        var chapter = await chapterRepository.FirstOrDefaultAsync(
            new ChapterWithPagesSpec(request.ChapterId), cancellationToken);

        if (chapter is null)
            throw new NotFoundException($"Chapter with ID '{request.ChapterId}' not found.");

        var userId = currentUser.GetUserId().ToString();

        var isEnrolled = await courseEnrollmentRepository.FirstOrDefaultAsync(
            new CourseEnrollmentByUserAndCourseSpec(userId, chapter.CourseId), cancellationToken) is not null;

        if (!isEnrolled)
            throw new ForbiddenException("You are not enrolled in the course this chapter belongs to.");

        // Chapters that are still staging (created by an in-progress PPTX import, or any
        // other unreviewed-content flow) are not published yet — even an enrolled learner
        // must not be able to play them (see PPTX import Task H)
        if (chapter.IsStaging)
            throw new ForbiddenException("This content has not been published yet.");

        var pageProgressList = await pageProgressRepository.ListAsync(
            new PageProgressListByUserAndChapterSpec(userId, request.ChapterId), cancellationToken);

        var progressByPageId = pageProgressList.ToDictionary(p => p.PageId);

        var pages = (chapter.Pages ?? [])
            .OrderBy(p => p.Order)
            .Select(p =>
            {
                progressByPageId.TryGetValue(p.Id, out var pageProgress);

                return new ChapterPlayerPageDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Order = p.Order,
                    ContentType = p.ContentType.ToString(),
                    Content = p.Content,
                    MediaUrl = p.MediaUrl,
                    IsImported = p.IsImported,
                    Completed = pageProgress?.Completed ?? false,
                    CompletedAt = pageProgress?.CompletedAt
                };
            })
            .ToList();

        var chapterProgress = await chapterProgressRepository.FirstOrDefaultAsync(
            new ChapterProgressByUserAndChapterSpec(userId, request.ChapterId), cancellationToken);

        return new ChapterPlayerDto
        {
            ChapterId = chapter.Id,
            ChapterTitle = chapter.Title,
            Pages = pages,
            PercentComplete = chapterProgress?.PercentComplete ?? 0,
            LastVisitedPageId = chapterProgress?.LastVisitedPageId
        };
    }
}
