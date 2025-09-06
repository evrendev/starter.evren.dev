using EvrenDev.Application.Multitenancy.Entities;
using EvrenDev.Application.Multitenancy.Interfaces;

namespace EvrenDev.Application.Multitenancy.Queries.Paginate;

public class PaginateTenantsFilter : PaginationFilter, IRequest<PaginationResponse<TenantDto>>
{
    public bool? ShowActiveItems { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class PaginateTenantsRequestHandler(ITenantService tenantService) : IRequestHandler<PaginateTenantsFilter, PaginationResponse<TenantDto>>
{
    public async Task<PaginationResponse<TenantDto>> Handle(PaginateTenantsFilter filter, CancellationToken cancellationToken)
    {
        return await tenantService.PaginatedListAsync(filter, cancellationToken);
    }
}
