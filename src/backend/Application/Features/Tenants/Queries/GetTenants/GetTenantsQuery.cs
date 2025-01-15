using EvrenDev.Application.Features.Tenants.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Tenants.Queries.GetTenants;

public class GetTenantsQuery : IRequest<Result<List<TenantDto>>>
{
    public string? SearchString { get; set; }
    public bool? IsActive { get; set; }
    public bool? ShowDeletedItems { get; set; } = false;
}

public class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, Result<List<TenantDto>>>
{
    private readonly ITenantDbContext _context;

    public GetTenantsQueryHandler(ITenantDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TenantDto>>> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tenants.AsQueryable();

        if (request.ShowDeletedItems.HasValue)
            query = query.IgnoreQueryFilters();

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchString))
        {
            query = query.Where(x =>
                x.Name!.Contains(request.SearchString) ||
                x.Host!.Contains(request.SearchString) ||
                x.AdminEmail!.Contains(request.SearchString));
        }

        var tenants = await query
            .AsNoTracking()
            .OrderBy(x => x.ValidUntil)
            .Select(x => new TenantDto
            {
                Id = x.Id,
                Name = x.Name,
                ConnectionString = x.ConnectionString,
                Host = x.Host,
                IsActive = x.IsActive,
                AdminEmail = x.AdminEmail,
                ValidUntil = x.ValidUntil,
                Description = x.Description,
                Deleted = x.Deleted
            })
            .ToListAsync(cancellationToken);

        return Result<List<TenantDto>>.Success(tenants);
    }
}
