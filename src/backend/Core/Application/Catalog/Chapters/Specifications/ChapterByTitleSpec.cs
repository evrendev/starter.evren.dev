using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Chapters.Specifications;

public class ChapterByTitleSpec : Specification<Chapter>, ISingleResultSpecification<Chapter>
{
    public ChapterByTitleSpec(string title) =>
        Query.Where(p => p.Title == title);
}
