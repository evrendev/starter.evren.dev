using EvrenDev.Application.Identity.Users.Entities;

namespace EvrenDev.Application.Identity.Users.Queries.UserRoles;

public class UserRolesRequest
{
    public List<UserRoleDto> UserRoles { get; set; } = new();
}
