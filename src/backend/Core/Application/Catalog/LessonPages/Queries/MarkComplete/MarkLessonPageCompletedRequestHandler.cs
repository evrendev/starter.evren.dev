using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Queries.MarkComplete;

public class MarkLessonPageCompletedRequest(Guid lessonPageId) : IRequest<bool>
{
    public Guid LessonPageId { get; set; } = lessonPageId;
}

public class MarkLessonPageCompletedRequestHandler(
    ICurrentUser currentUser)
    : IRequestHandler<MarkLessonPageCompletedRequest, bool>
{
    public Task<bool> Handle(MarkLessonPageCompletedRequest request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        return Task.FromResult(true);
    }
}
