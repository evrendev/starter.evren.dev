using EvrenDev.Application.Catalog.Chapters.Entities;
using EvrenDev.Application.Catalog.Chapters.Queries.Paginate;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Chapters.Specifications;

public class ChaptersBySearchRequestWithCoursesSpec : Specification<Chapter, ChapterDto>
{
    public ChaptersBySearchRequestWithCoursesSpec(PaginateChaptersFilter request)
    {
        Query.Include(p => p.Course)
            .Where(chapter =>
                (
                    !request.CourseId.HasValue
                    ||
                    chapter.CourseId.Equals(request.CourseId!.Value)
                )
                &&
                (
                    string.IsNullOrEmpty(request.Search)
                    ||
                    chapter.Title.ToLower().Contains(request.Search.ToLower())
                    ||
                    chapter.Course.Title.ToLower().Contains(request.Search.ToLower())
                    ||
                    (
                        chapter.Description != null
                        &&
                        chapter.Description.ToLower().Contains(request.Search.ToLower())
                    )
                )
            )
            .OrderBy(c => c.Title, !request.HasOrderBy())
            .PaginateBy(request);
    }
}
