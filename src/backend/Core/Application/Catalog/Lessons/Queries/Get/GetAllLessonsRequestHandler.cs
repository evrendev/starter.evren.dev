using EvrenDev.Application.Catalog.Lessons.Entities;
using EvrenDev.Application.Catalog.Lessons.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Lessons.Queries.Get;

public class GetAllLessonsRequest() : IRequest<List<LessonExportDto>>
{
}

public class GetAllLessonsRequestHandler(IRepository<Lesson> repository, IStringLocalizer<GetAllLessonsRequestHandler> localizer)
    : IRequestHandler<GetAllLessonsRequest, List<LessonExportDto>>
{
    public async Task<List<LessonExportDto>> Handle(GetAllLessonsRequest request, CancellationToken cancellationToken)
    {
        var lessons = await repository.ListAsync(new LessonsWithChaptersSpec(request), cancellationToken);

        if (lessons == null || !lessons.Any())
            throw new NotFoundException(string.Format(localizer["catalog.lessons.list.notfound"]));

        return lessons;
    }
}
