using EvrenDev.Application.Catalog.Chapters.Entities;
using EvrenDev.Application.Catalog.Chapters.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Chapters.Queries.Paginate;

public class PaginateChaptersFilter : PaginationFilter, IRequest<PaginationResponse<ChapterDto>>
{
    public Guid? CourseId { get; set; }
    public bool? Published { get; set; }
}

public class PaginateChaptersFilterHandler(IReadRepository<Chapter> repository) : IRequestHandler<PaginateChaptersFilter, PaginationResponse<ChapterDto>>
{
    public async Task<PaginationResponse<ChapterDto>> Handle(PaginateChaptersFilter request, CancellationToken cancellationToken)
    {
        var spec = new ChaptersBySearchRequestWithCoursesSpec(request);
        return await repository.PaginatedListAsync(spec, request.Page, request.ItemsPerPage, cancellationToken);
    }
}
