using EvrenDev.Application.Catalog.LessonPages.Entities;
using EvrenDev.Application.Catalog.LessonPages.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Queries.Get;

public class GetLessonPageRequest(Guid id) : IRequest<LessonPageDetailsDto>
{
    public Guid Id { get; set; } = id;
}

public class GetLessonPageRequestHandler(IRepository<LessonPage> repository, IStringLocalizer<GetLessonPageRequestHandler> localizer)
    : IRequestHandler<GetLessonPageRequest, LessonPageDetailsDto>
{
    public async Task<LessonPageDetailsDto> Handle(GetLessonPageRequest request, CancellationToken cancellationToken) =>
        await repository.FirstOrDefaultAsync(
            new LessonPageByIdWithLessonSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(localizer["catalog.lessonpages.get.notfound"], request.Id));
}
