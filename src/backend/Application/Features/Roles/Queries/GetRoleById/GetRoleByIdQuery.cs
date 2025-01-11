using EvrenDev.Application.Features.Roles.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Roles.Queries.GetRoleById;

public class GetRoleByIdQuery : IRequest<Result<RoleDto>>
{
    public string Id { get; set; } = string.Empty;
}

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<RoleDto>>
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public GetRoleByIdQueryHandler(
        RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.Roles.FirstOrDefaultAsync(role => role.Id == role.Id);
        if (role == null)
            return Result<RoleDto>.Failure("Role not found.");

        var roleDtos = new RoleDto()
        {
            Id = role.Id,
            Name = role.Name!
        };

        return Result<RoleDto>.Success(roleDtos);
    }
}
