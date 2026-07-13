using EvrenDev.Application.Catalog.LessonPages.Specifications;
using EvrenDev.Application.Catalog.Lessons.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Catalog.Events;

namespace EvrenDev.Application.Catalog.LessonPages.Queries.MarkComplete;

public class MarkLessonPageCompletedRequest(Guid lessonPageId) : IRequest<bool>
{
    public Guid LessonPageId { get; set; } = lessonPageId;
}

public class MarkLessonPageCompletedRequestHandler(
    IRepository<LessonPageProgress> repository,
    IReadRepository<LessonPage> lessonPageRepository,
    IReadRepository<CourseEnrollment> courseEnrollmentRepository,
    ICurrentUser currentUser)
    : IRequestHandler<MarkLessonPageCompletedRequest, bool>
{
    public async Task<bool> Handle(MarkLessonPageCompletedRequest request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId().ToString();
        var now = DateTime.UtcNow;

        var page = await lessonPageRepository.FirstOrDefaultAsync(
            new LessonPageWithLessonChapterSpec(request.LessonPageId), cancellationToken);

        if (page is null)
            throw new NotFoundException($"Lesson page with ID '{request.LessonPageId}' not found.");

        var isEnrolled = await courseEnrollmentRepository.FirstOrDefaultAsync(
            new CourseEnrollmentByUserAndCourseSpec(userId, page.Lesson.Chapter.CourseId), cancellationToken) is not null;

        if (!isEnrolled)
            throw new ForbiddenException("You are not enrolled in the course this lesson page belongs to.");

        var progress = await repository.FirstOrDefaultAsync(
            new LessonPageProgressByUserAndPageSpec(userId, request.LessonPageId), cancellationToken);

        if (progress is null)
        {
            progress = new LessonPageProgress
            {
                UserId = userId,
                LessonPageId = request.LessonPageId,
                Completed = true,
                CompletedAt = now,
                LastVisitedAt = now
            };

            progress.DomainEvents.Add(new LessonPageCompletedEvent(request.LessonPageId, userId));

            await repository.AddAsync(progress, cancellationToken);
        }
        else
        {
            progress.Completed = true;
            progress.CompletedAt ??= now;
            progress.LastVisitedAt = now;

            progress.DomainEvents.Add(new LessonPageCompletedEvent(request.LessonPageId, userId));

            await repository.UpdateAsync(progress, cancellationToken);
        }

        return true;
    }
}
