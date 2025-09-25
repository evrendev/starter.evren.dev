using EvrenDev.Application.Catalog.Lessons.Entities;
using EvrenDev.Application.Catalog.Lessons.Queries.Paginate;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Lessons.Specifications;

public class LessonsBySearchRequestWithChaptersSpec : Specification<Lesson, LessonDto>
{
    public LessonsBySearchRequestWithChaptersSpec(PaginateLessonsFilter request)
    {
        Query.Include(p => p.Chapter)
            .Where(lesson =>
                (
                    !request.ChapterId.HasValue
                    ||
                    lesson.ChapterId.Equals(request.ChapterId!.Value)
                )
                &&
                (
                    string.IsNullOrEmpty(request.Search)
                    ||
                    lesson.Title.ToLower().Contains(request.Search.ToLower())
                    ||
                    lesson.Chapter.Title.ToLower().Contains(request.Search.ToLower())
                    ||
                    (
                        lesson.Description != null
                        &&
                        lesson.Description.ToLower().Contains(request.Search.ToLower())
                    )
                )
            )
            .OrderBy(c => c.Title, !request.HasOrderBy())
            .PaginateBy(request);
    }
}
