using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Specifications;

public class ChapterWithPagesSpec : Specification<Chapter>, ISingleResultSpecification<Chapter>
{
    public ChapterWithPagesSpec(Guid chapterId) =>
        Query
            .Where(c => c.Id == chapterId)
            .Include(c => c.Pages);
}
