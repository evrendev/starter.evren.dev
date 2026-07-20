using EvrenDev.Application.Catalog.Pages.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Specifications;

public class PagesWithChaptersSpec : Specification<Page, PageExportDto>
{
    public PagesWithChaptersSpec() =>
        Query
            .Include(p => p.Chapter)
            .OrderBy(p => p.Order);
}
