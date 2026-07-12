using EvrenDev.Application.Catalog.LessonPages.Entities;
using EvrenDev.Application.Catalog.LessonPages.Queries.Paginate;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Specifications;

public class LessonPagesBySearchRequestWithLessonsSpec : Specification<LessonPage, LessonPageDto>
{
    public LessonPagesBySearchRequestWithLessonsSpec(PaginateLessonPagesFilter request)
    {
        Query.Include(p => p.Lesson)
            .Where(page =>
                (
                    !request.LessonId.HasValue
                    ||
                    page.LessonId.Equals(request.LessonId!.Value)
                )
                &&
                (
                    string.IsNullOrEmpty(request.Search)
                    ||
                    page.Title.ToLower().Contains(request.Search.ToLower())
                    ||
                    page.Lesson.Title.ToLower().Contains(request.Search.ToLower())
                )
            )
            .OrderBy(p => p.Order, !request.HasOrderBy())
            .PaginateBy(request);
    }
}
