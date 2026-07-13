using EvrenDev.Application.Catalog.LessonPages.Specifications;
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
    ICurrentUser currentUser)
    : IRequestHandler<MarkLessonPageCompletedRequest, bool>
{
    public async Task<bool> Handle(MarkLessonPageCompletedRequest request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId().ToString();
        var now = DateTime.UtcNow;

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
