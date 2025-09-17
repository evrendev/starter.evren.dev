namespace EvrenDev.Domain.Common.Events.Identity;

public abstract class ApplicationRoleEvent : DomainEvent
{
    protected ApplicationRoleEvent(string roleId, string roleName)
    {
        (RoleId, RoleName) = (roleId, roleName);
    }

    public string RoleId { get; set; } = default!;
    public string RoleName { get; set; } = default!;
}
