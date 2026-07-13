using EvrenDev.Application.Catalog.Lessons.Specifications;
using EvrenDev.Application.Common.Events;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Catalog.Events;

namespace EvrenDev.Application.Catalog.Lessons.EventHandlers;

public class RecalculateCourseProgressOnLessonCompletedHandler(
    IReadRepository<Lesson> lessonRepository,
    IReadRepository<LessonProgress> lessonProgressRepository,
    IRepository<CourseEnrollment> courseEnrollmentRepository)
    : EventNotificationHandler<LessonCompletedEvent>
{
    public override async Task Handle(LessonCompletedEvent @event, CancellationToken cancellationToken)
    {
        var lesson = await lessonRepository.FirstOrDefaultAsync(
            new LessonWithChapterSpec(@event.LessonId), cancellationToken);

        if (lesson is null)
            return;

        var courseId = lesson.Chapter.CourseId;

        var totalLessons = await lessonRepository.CountAsync(
            new LessonsByCourseSpec(courseId), cancellationToken);

        if (totalLessons == 0)
            return;

        var completedLessons = await lessonProgressRepository.CountAsync(
            new CompletedLessonProgressByUserAndCourseSpec(@event.UserId, courseId), cancellationToken);

        var percentComplete = (int)Math.Round(completedLessons * 100.0 / totalLessons);

        var enrollment = await courseEnrollmentRepository.FirstOrDefaultAsync(
            new CourseEnrollmentByUserAndCourseSpec(@event.UserId, courseId), cancellationToken);

        if (enrollment is null)
            return;

        enrollment.PercentComplete = percentComplete;

        await courseEnrollmentRepository.UpdateAsync(enrollment, cancellationToken);
    }
}
