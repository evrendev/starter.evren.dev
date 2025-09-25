using EvrenDev.Application.Catalog.Lessons.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Lessons.Specifications;

public class LessonByIdWithChapterSpec : Specification<Lesson, LessonDetailsDto>, ISingleResultSpecification<Lesson>
{
    public LessonByIdWithChapterSpec(Guid id) =>
        Query
            .Where(p => p.Id == id)
            .Include(p => p.Chapter);
}
