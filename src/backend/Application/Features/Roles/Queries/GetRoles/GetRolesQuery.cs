using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Roles.Queries.GetRoles;
public class GetRolesQuery : IRequest<Result<List<RoleDto>>>
{
}

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Result<List<RoleDto>>>
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IPermissionService _permissionService;

    public GetRolesQueryHandler(
        RoleManager<IdentityRole> roleManager,
        IPermissionService permissionService)
    {
        _roleManager = roleManager;
        _permissionService = permissionService;
    }

    public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var roleDtos = new List<RoleDto>();

        foreach (var role in roles)
        {
            var permissions = await _permissionService.GetRolePermissions(role.Id);
            roleDtos.Add(new RoleDto
            {
                Id = role.Id,
                Name = role.Name!,
                Permissions = permissions
            });
        }

        return Result<List<RoleDto>>.Success(roleDtos);
    }
}

public class RoleDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}