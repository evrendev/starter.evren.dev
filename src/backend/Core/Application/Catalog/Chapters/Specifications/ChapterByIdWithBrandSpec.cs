using EvrenDev.Application.Catalog.Chapters.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Chapters.Specifications;

public class ChapterByIdWithCategorySpec : Specification<Chapter, ChapterDto>, ISingleResultSpecification<Chapter>
{
    public ChapterByIdWithCategorySpec(Guid id) =>
        Query
            .Where(p => p.Id == id)
            .Include(p => p.Course);
}
