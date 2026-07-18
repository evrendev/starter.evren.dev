using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Chapters.Specifications;

// Lazy-lookup for the single "import staging" chapter a course accumulates
// imported (unreviewed) lessons into — see ImportLessonsFromPptxJob.
public class StagingChapterByCourseSpec : Specification<Chapter>, ISingleResultSpecification<Chapter>
{
    public StagingChapterByCourseSpec(Guid courseId) =>
        Query.Where(p => p.CourseId == courseId && p.IsStaging);
}
