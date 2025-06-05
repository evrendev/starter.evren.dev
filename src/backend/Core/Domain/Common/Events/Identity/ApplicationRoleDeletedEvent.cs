namespace EvrenDev.Domain.Common.Events.Identity;

public class ApplicationRoleDeletedEvent(string roleId, string roleName) : ApplicationRoleEvent(roleId, roleName)
{
    public bool PermissionsUpdated { get; set; }
}
