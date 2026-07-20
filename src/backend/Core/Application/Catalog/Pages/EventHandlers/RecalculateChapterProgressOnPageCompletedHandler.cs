using EvrenDev.Application.Catalog.Pages.Specifications;
using EvrenDev.Application.Common.Events;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Catalog.Events;

namespace EvrenDev.Application.Catalog.Pages.EventHandlers;

public class RecalculateChapterProgressOnPageCompletedHandler(
    IReadRepository<Page> pageRepository,
    IReadRepository<PageProgress> pageProgressRepository,
    IRepository<ChapterProgress> chapterProgressRepository)
    : EventNotificationHandler<PageCompletedEvent>
{
    public override async Task Handle(PageCompletedEvent @event, CancellationToken cancellationToken)
    {
        var page = await pageRepository.GetByIdAsync(@event.PageId, cancellationToken);
        if (page is null)
            return;

        var chapterId = page.ChapterId;

        var totalPages = await pageRepository.CountAsync(
            new PagesByChapterSpec(chapterId), cancellationToken);

        if (totalPages == 0)
            return;

        var completedPages = await pageProgressRepository.CountAsync(
            new CompletedPageProgressByUserAndChapterSpec(@event.UserId, chapterId), cancellationToken);

        var percentComplete = (int)Math.Round(completedPages * 100.0 / totalPages);
        var status = percentComplete >= 100
            ? ProgressStatus.Completed
            : percentComplete > 0
                ? ProgressStatus.InProgress
                : ProgressStatus.NotStarted;

        var chapterProgress = await chapterProgressRepository.FirstOrDefaultAsync(
            new ChapterProgressByUserAndChapterSpec(@event.UserId, chapterId), cancellationToken);

        var wasAlreadyCompleted = chapterProgress?.Status == ProgressStatus.Completed;

        if (chapterProgress is null)
        {
            chapterProgress = new ChapterProgress
            {
                UserId = @event.UserId,
                ChapterId = chapterId,
                Status = status,
                PercentComplete = percentComplete,
                LastVisitedPageId = @event.PageId,
                CompletedAt = status == ProgressStatus.Completed ? DateTime.UtcNow : null
            };

            if (status == ProgressStatus.Completed)
                chapterProgress.DomainEvents.Add(new ChapterCompletedEvent(chapterId, @event.UserId));

            await chapterProgressRepository.AddAsync(chapterProgress, cancellationToken);
        }
        else
        {
            chapterProgress.Status = status;
            chapterProgress.PercentComplete = percentComplete;
            chapterProgress.LastVisitedPageId = @event.PageId;

            if (status == ProgressStatus.Completed && !wasAlreadyCompleted)
            {
                chapterProgress.CompletedAt = DateTime.UtcNow;
                chapterProgress.DomainEvents.Add(new ChapterCompletedEvent(chapterId, @event.UserId));
            }

            await chapterProgressRepository.UpdateAsync(chapterProgress, cancellationToken);
        }
    }
}
