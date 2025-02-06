using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Users.Queries.GetUsers;

public class GetUsersQuery : IRequest<Result<List<UserDto>>>
{
    public bool? ShowDeletedItems { get; set; } = false;
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<List<UserDto>>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUsersQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<List<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _userManager.Users.AsQueryable();

        if (!request.ShowDeletedItems.HasValue)
            query = query.Where(u => !u.Deleted);

        var users = await query.ToListAsync(cancellationToken);
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var permissions = claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();

            userDtos.Add(new UserDto
            {
                Id = user.Id,
                TenantId = user.TenantId,
                Gender = user.Gender?.Code,
                Email = user.Email!,
                FirstName = user.FirstName!,
                LastName = user.LastName!,
                FullName = user.FullName,
                Image = user.Image ?? string.Empty,
                JobTitle = user.JobTitle ?? string.Empty,
                Language = user.Language?.Code,
                Permissions = permissions
            });
        }

        return Result<List<UserDto>>.Success(userDtos);
    }
}
