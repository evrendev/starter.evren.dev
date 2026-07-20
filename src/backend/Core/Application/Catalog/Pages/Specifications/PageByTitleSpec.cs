using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Specifications;

public class PageByTitleSpec : Specification<Page>, ISingleResultSpecification<Page>
{
    public PageByTitleSpec(string title) =>
        Query.Where(p => p.Title == title);
}
