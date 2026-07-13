using EvrenDev.Application.Catalog.LessonPages.Specifications;
using EvrenDev.Application.Common.Events;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Catalog.Events;

namespace EvrenDev.Application.Catalog.LessonPages.EventHandlers;

public class RecalculateLessonProgressOnPageCompletedHandler(
    IReadRepository<LessonPage> lessonPageRepository,
    IReadRepository<LessonPageProgress> lessonPageProgressRepository,
    IRepository<LessonProgress> lessonProgressRepository)
    : EventNotificationHandler<LessonPageCompletedEvent>
{
    public override async Task Handle(LessonPageCompletedEvent @event, CancellationToken cancellationToken)
    {
        var page = await lessonPageRepository.GetByIdAsync(@event.LessonPageId, cancellationToken);
        if (page is null)
            return;

        var lessonId = page.LessonId;

        var totalPages = await lessonPageRepository.CountAsync(
            new LessonPagesByLessonSpec(lessonId), cancellationToken);

        if (totalPages == 0)
            return;

        var completedPages = await lessonPageProgressRepository.CountAsync(
            new CompletedLessonPageProgressByUserAndLessonSpec(@event.UserId, lessonId), cancellationToken);

        var percentComplete = (int)Math.Round(completedPages * 100.0 / totalPages);
        var status = percentComplete >= 100
            ? ProgressStatus.Completed
            : percentComplete > 0
                ? ProgressStatus.InProgress
                : ProgressStatus.NotStarted;

        var lessonProgress = await lessonProgressRepository.FirstOrDefaultAsync(
            new LessonProgressByUserAndLessonSpec(@event.UserId, lessonId), cancellationToken);

        var wasAlreadyCompleted = lessonProgress?.Status == ProgressStatus.Completed;

        if (lessonProgress is null)
        {
            lessonProgress = new LessonProgress
            {
                UserId = @event.UserId,
                LessonId = lessonId,
                Status = status,
                PercentComplete = percentComplete,
                LastVisitedPageId = @event.LessonPageId,
                CompletedAt = status == ProgressStatus.Completed ? DateTime.UtcNow : null
            };

            if (status == ProgressStatus.Completed)
                lessonProgress.DomainEvents.Add(new LessonCompletedEvent(lessonId, @event.UserId));

            await lessonProgressRepository.AddAsync(lessonProgress, cancellationToken);
        }
        else
        {
            lessonProgress.Status = status;
            lessonProgress.PercentComplete = percentComplete;
            lessonProgress.LastVisitedPageId = @event.LessonPageId;

            if (status == ProgressStatus.Completed && !wasAlreadyCompleted)
            {
                lessonProgress.CompletedAt = DateTime.UtcNow;
                lessonProgress.DomainEvents.Add(new LessonCompletedEvent(lessonId, @event.UserId));
            }

            await lessonProgressRepository.UpdateAsync(lessonProgress, cancellationToken);
        }
    }
}
