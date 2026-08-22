using EvrenDev.Application.Catalog.CourseEnrollments.Specifications;
using EvrenDev.Application.Catalog.Pages.Entities;
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
    // Null/empty means the legacy "(richtig)" Content pattern is still in play for
    // this page — the player falls back to parsing Content in that case (see
    // QuizContent.vue). Non-empty means this page uses the structural Quiz model.
    // IsCorrect is sent to the client as-is for now — a follow-up should hide it
    // until the learner actually selects an option, since a raw network-tab read
    // currently exposes the answer before they attempt the question (flagged in
    // Task N0/N1, out of scope here).
    public List<QuestionDto>? Questions { get; set; }
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

        var pages = chapter.Pages
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
                    CompletedAt = pageProgress?.CompletedAt,
                    Questions = p.Questions.Count == 0
                        ? null
                        : p.Questions
                            .OrderBy(q => q.Order)
                            .Select(q => new QuestionDto
                            {
                                Prompt = q.Prompt,
                                Order = q.Order,
                                Options = q.Options
                                    .OrderBy(o => o.Order)
                                    .Select(o => new OptionDto
                                    {
                                        Label = o.Label,
                                        IsCorrect = o.IsCorrect,
                                        Order = o.Order
                                    })
                                    .ToList()
                            })
                            .ToList()
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
