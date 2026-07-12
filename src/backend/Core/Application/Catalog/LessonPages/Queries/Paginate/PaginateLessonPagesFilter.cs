using EvrenDev.Application.Catalog.LessonPages.Entities;
using EvrenDev.Application.Catalog.LessonPages.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Queries.Paginate;

public class PaginateLessonPagesFilter : PaginationFilter, IRequest<PaginationResponse<LessonPageDto>>
{
    public Guid? LessonId { get; set; }
}

public class PaginateLessonPagesFilterHandler(IReadRepository<LessonPage> repository) : IRequestHandler<PaginateLessonPagesFilter, PaginationResponse<LessonPageDto>>
{
    public async Task<PaginationResponse<LessonPageDto>> Handle(PaginateLessonPagesFilter request, CancellationToken cancellationToken)
    {
        var spec = new LessonPagesBySearchRequestWithLessonsSpec(request);
        return await repository.PaginatedListAsync(spec, request.Page, request.ItemsPerPage, cancellationToken);
    }
}
