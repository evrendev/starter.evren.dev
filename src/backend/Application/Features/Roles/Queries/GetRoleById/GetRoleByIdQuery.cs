using EvrenDev.Application.Common.Models;
using EvrenDev.Application.Features.Roles.Queries.GetRoles;
using MediatR;

namespace EvrenDev.Application.Features.Roles.Queries.GetRoleById;

public class GetRoleByIdQuery : IRequest<Result<RoleDto>>
{
    public string Id { get; set; } = string.Empty;
}