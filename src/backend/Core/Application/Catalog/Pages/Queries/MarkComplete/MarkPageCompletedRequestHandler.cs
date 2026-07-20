using EvrenDev.Application.Catalog.CourseEnrollments.Specifications;
using EvrenDev.Application.Catalog.Pages.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Catalog.Events;

namespace EvrenDev.Application.Catalog.Pages.Queries.MarkComplete;

public class MarkPageCompletedRequest(Guid pageId) : IRequest<bool>
{
    public Guid PageId { get; set; } = pageId;
}

public class MarkPageCompletedRequestHandler(
    IRepository<PageProgress> repository,
    IReadRepository<Page> pageRepository,
    IReadRepository<CourseEnrollment> courseEnrollmentRepository,
    ICurrentUser currentUser)
    : IRequestHandler<MarkPageCompletedRequest, bool>
{
    public async Task<bool> Handle(MarkPageCompletedRequest request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId().ToString();
        var now = DateTime.UtcNow;

        var page = await pageRepository.FirstOrDefaultAsync(
            new PageWithChapterSpec(request.PageId), cancellationToken);

        if (page is null)
            throw new NotFoundException($"Page with ID '{request.PageId}' not found.");

        var isEnrolled = await courseEnrollmentRepository.FirstOrDefaultAsync(
            new CourseEnrollmentByUserAndCourseSpec(userId, page.Chapter.CourseId), cancellationToken) is not null;

        if (!isEnrolled)
            throw new ForbiddenException("You are not enrolled in the course this page belongs to.");

        // Same staging gate as GetChapterPlayerRequestHandler: a student must not be able
        // to mark a not-yet-published (staging) page as completed by calling this API
        // directly, even if they never saw it rendered anywhere (see PPTX import Task H)
        if (page.Chapter.IsStaging)
            throw new ForbiddenException("This content has not been published yet.");

        var progress = await repository.FirstOrDefaultAsync(
            new PageProgressByUserAndPageSpec(userId, request.PageId), cancellationToken);

        if (progress is null)
        {
            progress = new PageProgress
            {
                UserId = userId,
                PageId = request.PageId,
                Completed = true,
                CompletedAt = now,
                LastVisitedAt = now
            };

            progress.DomainEvents.Add(new PageCompletedEvent(request.PageId, userId));

            await repository.AddAsync(progress, cancellationToken);
        }
        else
        {
            progress.Completed = true;
            progress.CompletedAt ??= now;
            progress.LastVisitedAt = now;

            progress.DomainEvents.Add(new PageCompletedEvent(request.PageId, userId));

            await repository.UpdateAsync(progress, cancellationToken);
        }

        return true;
    }
}
