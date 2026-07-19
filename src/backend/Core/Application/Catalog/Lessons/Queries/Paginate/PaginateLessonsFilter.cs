using EvrenDev.Application.Catalog.Lessons.Entities;
using EvrenDev.Application.Catalog.Lessons.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Lessons.Queries.Paginate;

public class PaginateLessonsFilter : PaginationFilter, IRequest<PaginationResponse<LessonDto>>
{
    public Guid? ChapterId { get; set; }
    // On-toggle filters (only meaningful when true; left null/false means "no filter") —
    // surface the admin "Unclassified only" / "Needs review only" list toggles
    public bool? IsStaging { get; set; }
    public bool? NeedsReview { get; set; }
}

public class PaginateLessonsFilterHandler(IReadRepository<Lesson> repository) : IRequestHandler<PaginateLessonsFilter, PaginationResponse<LessonDto>>
{
    public async Task<PaginationResponse<LessonDto>> Handle(PaginateLessonsFilter request, CancellationToken cancellationToken)
    {
        var spec = new LessonsBySearchRequestWithChaptersSpec(request);
        return await repository.PaginatedListAsync(spec, request.Page, request.ItemsPerPage, cancellationToken);
    }
}
