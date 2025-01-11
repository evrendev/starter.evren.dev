using EvrenDev.Application.Features.Tenants.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Tenants.Queries.GetTenants;

public class GetTenantsQuery : IRequest<Result<List<TenantDto>>>
{
    public string? SearchString { get; set; }
    public bool? IsActive { get; set; }
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

        if (!string.IsNullOrWhiteSpace(request.SearchString))
        {
            query = query.Where(x =>
                x.Name!.Contains(request.SearchString) ||
                x.Host!.Contains(request.SearchString));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        var tenants = await query
            .OrderBy(x => x.Name)
            .Select(x => new TenantDto
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return Result<List<TenantDto>>.Success(tenants);
    }
}
