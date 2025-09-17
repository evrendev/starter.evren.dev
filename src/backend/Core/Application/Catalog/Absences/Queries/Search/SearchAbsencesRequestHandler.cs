using EvrenDev.Application.Catalog.Absences.Entities;
using EvrenDev.Application.Catalog.Absences.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Absences.Queries.Search;

public class SearchAbsencesRequest : PaginationFilter, IRequest<PaginationResponse<AbsenceDto>>
{
}

public class SearchAbsencesRequestHandler
    (IReadRepository<Absence> repository) : IRequestHandler<SearchAbsencesRequest, PaginationResponse<AbsenceDto>>
{
    public async Task<PaginationResponse<AbsenceDto>> Handle(SearchAbsencesRequest request,
        CancellationToken cancellationToken)
    {
        var spec = new AbsencesBySearchRequestSpec(request);
        return await repository.PaginatedListAsync(spec, request.Page, request.ItemsPerPage, cancellationToken);
    }
}
