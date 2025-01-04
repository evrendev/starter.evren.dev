using EvrenDev.Application.Common.Models;
using MediatR;

namespace EvrenDev.Application.Features.Roles.Commands.UpdateRolePermissions;

public class UpdateRolePermissionsCommand : IRequest<Result<bool>>
{
    public string RoleId { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}