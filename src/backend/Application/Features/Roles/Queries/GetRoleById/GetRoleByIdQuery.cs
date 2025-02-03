using EvrenDev.Application.Features.Roles.Model;
using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Roles.Queries.GetRoleById;

public class GetRoleByIdQuery : IRequest<Result<RoleDto>>
{
    public Guid Id { get; set; }
}

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<RoleDto>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public GetRoleByIdQueryHandler(
        RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.Roles.FirstOrDefaultAsync(role => role.Id == request.Id);
        if (role == null)
            return Result<RoleDto>.Failure("Role not found.");

        var roleDtos = new RoleDto()
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description
        };

        return Result<RoleDto>.Success(roleDtos);
    }
}
