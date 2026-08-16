using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Specifications;

// Loads Questions/Options for a Page so ReplaceQuestions can safely clear the
// tracked collection (an unloaded navigation would silently no-op on Clear —
// EF would never see the old rows to delete them). Used by the Update handler
// only when the request actually carries a Questions list.
public class PageWithQuestionsSpec : Specification<Page>, ISingleResultSpecification<Page>
{
    public PageWithQuestionsSpec(Guid pageId) =>
        Query
            .Where(p => p.Id == pageId)
            .Include(p => p.Questions)
            .ThenInclude(q => q.Options);
}
