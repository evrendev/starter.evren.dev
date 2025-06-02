namespace EvrenDev.Domain.Identity;

public class ApplicationRoleDeletedEvent(string roleId, string roleName) : ApplicationRoleEvent(roleId, roleName)
{
    public bool PermissionsUpdated { get; set; }
}
