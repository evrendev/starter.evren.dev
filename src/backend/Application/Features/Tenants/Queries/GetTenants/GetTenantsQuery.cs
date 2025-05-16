using EvrenDev.Application.Features.Tenants.Models;
using EvrenDev.Domain.Entities.Tenant;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Tenants.Queries.GetTenants;

public class GetTenantsQuery : IRequest<Result<PaginatedList<BasicTenantDto>>>
{
    public string? Search { get; init; }
    public bool? ShowActiveItems { get; init; }
    public bool? ShowDeletedItems { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int Page { get; init; } = 1;
    public int ItemsPerPage { get; init; } = 25;
    public string? SortBy { get; init; }
    public string? SortDesc { get; init; }
}

public class GetTenantsQueryValidator : AbstractValidator<GetTenantsQuery>
{
    private readonly IStringLocalizer<GetTenantsQueryValidator> _localizer;

    public GetTenantsQueryValidator(IStringLocalizer<GetTenantsQueryValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Page)
            .NotEmpty().WithMessage(_localizer["api.tenants.activate.page.required"])
            .GreaterThan(0).WithMessage(_localizer["api.tenants.activate.page.greater-than-zero"]);
    }
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

        if (request.ShowActiveItems.HasValue)
            query = query.Where(x => x.IsActive == request.ShowActiveItems.Value);

        if (request.StartDate != null)
            query = query.Where(entity => entity.ValidUntil >= request.StartDate);

        if (request.EndDate != null)
            query = query.Where(entity => entity.ValidUntil <= request.EndDate);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(x =>
                x.Name!.Contains(request.Search) ||
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

    private static IQueryable<AppTenantInfo> ApplySorting(IQueryable<AppTenantInfo> query, string sortBy, bool sortDesc)
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
