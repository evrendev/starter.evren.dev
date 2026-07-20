using EvrenDev.Application.Catalog.Chapters.Specifications;
using EvrenDev.Application.Catalog.CourseEnrollments.Specifications;
using EvrenDev.Application.Common.Events;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Catalog.Events;

namespace EvrenDev.Application.Catalog.Chapters.EventHandlers;

public class RecalculateCourseProgressOnChapterCompletedHandler(
    IReadRepository<Chapter> chapterRepository,
    IReadRepository<ChapterProgress> chapterProgressRepository,
    IRepository<CourseEnrollment> courseEnrollmentRepository)
    : EventNotificationHandler<ChapterCompletedEvent>
{
    public override async Task Handle(ChapterCompletedEvent @event, CancellationToken cancellationToken)
    {
        var chapter = await chapterRepository.GetByIdAsync(@event.ChapterId, cancellationToken);

        if (chapter is null)
            return;

        var courseId = chapter.CourseId;

        var totalChapters = await chapterRepository.CountAsync(
            new ChaptersByCourseSpec(courseId), cancellationToken);

        if (totalChapters == 0)
            return;

        var completedChapters = await chapterProgressRepository.CountAsync(
            new CompletedChapterProgressByUserAndCourseSpec(@event.UserId, courseId), cancellationToken);

        var percentComplete = (int)Math.Round(completedChapters * 100.0 / totalChapters);

        var enrollment = await courseEnrollmentRepository.FirstOrDefaultAsync(
            new CourseEnrollmentByUserAndCourseSpec(@event.UserId, courseId), cancellationToken);

        if (enrollment is null)
            return;

        enrollment.PercentComplete = percentComplete;

        await courseEnrollmentRepository.UpdateAsync(enrollment, cancellationToken);
    }
}
