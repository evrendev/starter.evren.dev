using EvrenDev.Application.Common.Models;
using MediatR;

namespace EvrenDev.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommand : IRequest<Result<bool>>
{
    public string Id { get; set; } = string.Empty;
}