using EvrenDev.Application.Features.Roles.Model;
using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Roles.Queries.GetRoles;
public class GetRolesQuery : IRequest<Result<PaginatedList<RoleDto>>>
{
    public string? Search { get; set; }
    public int Page { get; init; } = 1;
    public int ItemsPerPage { get; init; } = 25;
}

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Result<PaginatedList<RoleDto>>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public GetRolesQueryHandler(
        RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result<PaginatedList<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var query = _roleManager.Roles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(x =>
                x.Name!.Contains(request.Search));

        var dtoQuery = query.Select(role => new RoleDto
        {
            Id = role.Id,
            Name = role.Name!
        });

        var paginatedList = await PaginatedList<RoleDto>.CreateAsync(
            dtoQuery,
            request.Page,
            request.ItemsPerPage);

        return Result<PaginatedList<RoleDto>>.Success(paginatedList);
    }
}
