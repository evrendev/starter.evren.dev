using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Queries.Delete;

public class DeleteLessonPageRequest(Guid id) : IRequest<Guid>
{
    public Guid Id { get; set; } = id;
}

public class DeleteLessonPageRequestHandler(IRepository<LessonPage> repository, IStringLocalizer<DeleteLessonPageRequestHandler> localizer)
    : IRequestHandler<DeleteLessonPageRequest, Guid>
{
    public async Task<Guid> Handle(DeleteLessonPageRequest request, CancellationToken cancellationToken)
    {
        var lessonPage = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = lessonPage ?? throw new NotFoundException(localizer["catalog.lessonpages.delete.notfound"]);

        await repository.DeleteAsync(lessonPage, cancellationToken);

        return request.Id;
    }
}
