using EvrenDev.Application.Catalog.LessonPages.Entities;
using EvrenDev.Application.Catalog.LessonPages.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Queries.Get;

public class GetAllLessonPagesRequest() : IRequest<List<LessonPageExportDto>>
{
}

public class GetAllLessonPagesRequestHandler(IRepository<LessonPage> repository, IStringLocalizer<GetAllLessonPagesRequestHandler> localizer)
    : IRequestHandler<GetAllLessonPagesRequest, List<LessonPageExportDto>>
{
    public async Task<List<LessonPageExportDto>> Handle(GetAllLessonPagesRequest request, CancellationToken cancellationToken)
    {
        var lessonPages = await repository.ListAsync(new LessonPagesWithLessonsSpec(), cancellationToken);

        if (lessonPages == null || !lessonPages.Any())
            throw new NotFoundException(string.Format(localizer["catalog.lessonpages.list.notfound"]));

        return lessonPages;
    }
}
