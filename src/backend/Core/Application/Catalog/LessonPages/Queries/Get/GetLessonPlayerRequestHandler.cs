using EvrenDev.Application.Catalog.LessonPages.Specifications;
using EvrenDev.Application.Catalog.Lessons.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Queries.Get;

public class LessonPlayerPageDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public int Order { get; set; }
    public string ContentType { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string? MediaUrl { get; set; }
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class GetLessonPlayerRequest(Guid lessonId) : IRequest<LessonPlayerDto>
{
    public Guid LessonId { get; set; } = lessonId;
}

public class LessonPlayerDto
{
    public Guid LessonId { get; set; }
    public string LessonTitle { get; set; } = default!;
    public List<LessonPlayerPageDto> Pages { get; set; } = [];
    public int PercentComplete { get; set; }
    public Guid? LastVisitedPageId { get; set; }
}

public class GetLessonPlayerRequestHandler(
    IReadRepository<Lesson> lessonRepository,
    IReadRepository<LessonPageProgress> lessonPageProgressRepository,
    IReadRepository<LessonProgress> lessonProgressRepository,
    IReadRepository<CourseEnrollment> courseEnrollmentRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetLessonPlayerRequest, LessonPlayerDto>
{
    public async Task<LessonPlayerDto> Handle(GetLessonPlayerRequest request, CancellationToken cancellationToken)
    {
        var lesson = await lessonRepository.FirstOrDefaultAsync(
            new LessonWithPagesSpec(request.LessonId), cancellationToken);

        if (lesson is null)
            throw new NotFoundException($"Lesson with ID '{request.LessonId}' not found.");

        var userId = currentUser.GetUserId().ToString();

        var isEnrolled = await courseEnrollmentRepository.FirstOrDefaultAsync(
            new CourseEnrollmentByUserAndCourseSpec(userId, lesson.Chapter.CourseId), cancellationToken) is not null;

        if (!isEnrolled)
            throw new ForbiddenException("You are not enrolled in the course this lesson belongs to.");

        var pageProgressList = await lessonPageProgressRepository.ListAsync(
            new LessonPageProgressListByUserAndLessonSpec(userId, request.LessonId), cancellationToken);

        var progressByPageId = pageProgressList.ToDictionary(p => p.LessonPageId);

        var pages = lesson.Pages
            .OrderBy(p => p.Order)
            .Select(p =>
            {
                progressByPageId.TryGetValue(p.Id, out var pageProgress);

                return new LessonPlayerPageDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Order = p.Order,
                    ContentType = p.ContentType.ToString(),
                    Content = p.Content,
                    MediaUrl = p.MediaUrl,
                    Completed = pageProgress?.Completed ?? false,
                    CompletedAt = pageProgress?.CompletedAt
                };
            })
            .ToList();

        var lessonProgress = await lessonProgressRepository.FirstOrDefaultAsync(
            new LessonProgressByUserAndLessonSpec(userId, request.LessonId), cancellationToken);

        return new LessonPlayerDto
        {
            LessonId = lesson.Id,
            LessonTitle = lesson.Title,
            Pages = pages,
            PercentComplete = lessonProgress?.PercentComplete ?? 0,
            LastVisitedPageId = lessonProgress?.LastVisitedPageId
        };
    }
}
