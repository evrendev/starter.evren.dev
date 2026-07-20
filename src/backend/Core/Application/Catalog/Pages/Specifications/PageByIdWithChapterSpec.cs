using EvrenDev.Application.Catalog.Pages.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Specifications;

public class PageByIdWithChapterSpec : Specification<Page, PageDetailsDto>, ISingleResultSpecification<Page>
{
    public PageByIdWithChapterSpec(Guid id) =>
        Query
            .Where(p => p.Id == id)
            .Include(p => p.Chapter);
}
