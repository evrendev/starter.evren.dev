using EvrenDev.Application.Features.Users.Models;
using EvrenDev.Domain.Entities.Identity;
using EvrenDev.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Users.Queries.GetUsers;

public class GetUsersQuery : IRequest<Result<PaginatedList<BasicUserDto>>>
{
    public bool? ShowDeletedItems { get; set; } = false;
    public string? Search { get; set; }
    public bool? IsDeleted { get; set; } = false;
    public DateTime? StartDate { get; init; } = null;
    public DateTime? EndDate { get; init; } = null;
    public int Page { get; init; } = 1;
    public int ItemsPerPage { get; init; } = 25;
    public string? SortBy { get; init; }
    public string? SortDesc { get; init; }
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PaginatedList<BasicUserDto>>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly IStringLocalizer<GetUsersQueryHandler> _localizer;

    public GetUsersQueryHandler(UserManager<ApplicationUser> userManager,
        IStringLocalizer<GetUsersQueryHandler> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;
    }

    public async Task<Result<PaginatedList<BasicUserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _userManager.Users.AsQueryable();

        if (!request.ShowDeletedItems.HasValue)
            query = query.Where(u => !u.Deleted);

        if (request.IsDeleted.HasValue)
            query = query.IgnoreQueryFilters();

        if (request.StartDate != null)
            query = query.Where(entity => entity.CreatedTime >= request.StartDate);

        if (request.EndDate != null)
            query = query.Where(entity => entity.CreatedTime <= request.EndDate);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(x =>
                x.FirstName!.Contains(request.Search) ||
                x.LastName!.Contains(request.Search) ||
                x.JobTitle!.Contains(request.Search) ||
                x.Email!.Contains(request.Search));

        query = !string.IsNullOrEmpty(request.SortBy) && !string.IsNullOrEmpty(request.SortDesc)
            ? ApplySorting(query, request.SortBy, request.SortDesc == "desc")
            : query.OrderByDescending(x => x.CreatedTime);

        var users = await query.ToListAsync(cancellationToken);
        var userDtos = new List<BasicUserDto>();
        var dtoQuery = query.Select(user => new BasicUserDto
        {
            Id = user.Id,
            Tenant = user.TenantId.ToString(),
            Gender = _localizer[$"api.predefined-values.gender.{user.Gender!.Code}"],
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            TwoFactorEnabled = user.TwoFactorEnabled
        });

        var paginatedList = await PaginatedList<BasicUserDto>.CreateAsync(
            dtoQuery,
            request.Page,
            request.ItemsPerPage);

        return Result<PaginatedList<BasicUserDto>>.Success(paginatedList);
    }

    private static IQueryable<ApplicationUser> ApplySorting(IQueryable<ApplicationUser> query, string sortBy, bool sortDesc)
    {
        return sortBy.ToLower() switch
        {
            "firstName" => sortDesc
                ? query.OrderByDescending(x => x.FirstName)
                : query.OrderBy(x => x.FirstName),
            "lastName" => sortDesc
                ? query.OrderByDescending(x => x.LastName)
                : query.OrderBy(x => x.LastName),
            "email" => sortDesc
                ? query.OrderByDescending(x => x.Email)
                : query.OrderBy(x => x.Email),
            _ => query.OrderByDescending(x => x.CreatedTime) // Default sorting
        };
    }
}
