using EvrenDev.Application.Features.Tenants.Models;
using EvrenDev.Domain.Entities.Tenant;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Tenants.Queries.GetTenants;

public class GetTenantsQuery : IRequest<Result<PaginatedList<BasicTenantDto>>>
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public bool? ShowDeletedItems { get; set; } = false;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int ItemsPerPage { get; set; } = 25;
    public string? SortBy { get; set; }
    public string? SortDesc { get; set; }
}

public class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, Result<PaginatedList<BasicTenantDto>>>
{
    private readonly ITenantDbContext _context;

    public GetTenantsQueryHandler(ITenantDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<BasicTenantDto>>> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tenants.AsQueryable();

        if (request.ShowDeletedItems.HasValue)
            query = query.IgnoreQueryFilters().Where(x => x.Deleted == request.ShowDeletedItems.Value);

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        if (request.StartDate != null)
            query = query.Where(entity => entity.ValidUntil >= request.StartDate);

        if (request.EndDate != null)
            query = query.Where(entity => entity.ValidUntil <= request.EndDate);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(x =>
                x.Name!.Contains(request.Search) ||
                x.Host!.Contains(request.Search) ||
                x.AdminEmail!.Contains(request.Search));

        query = !string.IsNullOrEmpty(request.SortBy) && !string.IsNullOrEmpty(request.SortDesc)
            ? ApplySorting(query, request.SortBy, request.SortDesc == "desc")
            : query.OrderByDescending(x => x.ValidUntil);

        var dtoQuery = query.Select(tenant => new BasicTenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            IsActive = tenant.IsActive,
            Deleted = tenant.Deleted,
            AdminEmail = tenant.AdminEmail,
            ValidUntil = DateTimeDto.Create.FromLocal(tenant.ValidUntil)
        });

        var paginatedList = await PaginatedList<BasicTenantDto>.CreateAsync(
            dtoQuery,
            request.Page,
            request.ItemsPerPage);

        return Result<PaginatedList<BasicTenantDto>>.Success(paginatedList);
    }

    private static IQueryable<TenantEntity> ApplySorting(IQueryable<TenantEntity> query, string sortBy, bool sortDesc)
    {
        return sortBy.ToLower() switch
        {
            "name" => sortDesc
                ? query.OrderByDescending(x => x.Name)
                : query.OrderBy(x => x.Name),
            "validuntil" => sortDesc
                ? query.OrderByDescending(x => x.ValidUntil)
                : query.OrderBy(x => x.ValidUntil),
            _ => query.OrderByDescending(x => x.ValidUntil)
        };
    }
}
