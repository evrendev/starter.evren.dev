using EvrenDev.Application.Catalog.Absences.Entities;
using EvrenDev.Application.Catalog.Absences.Queries.Search;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Absences.Specifications;

public class AbsencesBySearchRequestSpec : EntitiesByPaginationFilterSpec<Absence, AbsenceDto>
{
    public AbsencesBySearchRequestSpec(SearchAbsencesRequest request)
        : base(request)
    {
        Query.OrderBy(c => c.StartDate, !request.HasOrderBy());
    }
}
