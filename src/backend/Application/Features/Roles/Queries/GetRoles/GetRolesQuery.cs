using EvrenDev.Application.Features.Roles.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Roles.Queries.GetRoles;
public class GetRolesQuery : IRequest<Result<List<RoleDto>>>
{
}

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Result<List<RoleDto>>>
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public GetRolesQueryHandler(
        RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var roleDtos = new List<RoleDto>();

        foreach (var role in roles)
        {
            roleDtos.Add(new RoleDto
            {
                Id = role.Id,
                Name = role.Name!
            });
        }

        return Result<List<RoleDto>>.Success(roleDtos);
    }
}
