using EvrenDev.Application.Catalog.Chapters.Entities;
using EvrenDev.Application.Catalog.Chapters.Queries.Paginate;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Chapters.Specifications;

public class ChaptersBySearchRequestWithCoursesSpec : Specification<Chapter, ChapterDto>
{
    // includeStaging is computed server-side from the caller's role (Admin vs Basic) in
    // PaginateChaptersFilterHandler, never from client input — this is what actually keeps
    // staging/unreviewed chapters out of student-facing listings (e.g. my-courses.vue),
    // regardless of what query params a non-admin caller sends (see PPTX import Task H)
    public ChaptersBySearchRequestWithCoursesSpec(PaginateChaptersFilter request, bool includeStaging)
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
                &&
                (
                    includeStaging || !chapter.IsStaging
                )
            )
            .OrderBy(c => c.Title, !request.HasOrderBy())
            .PaginateBy(request);
    }
}
