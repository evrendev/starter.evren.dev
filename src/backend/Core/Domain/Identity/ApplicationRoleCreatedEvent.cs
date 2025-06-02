namespace EvrenDev.Domain.Identity;

public class ApplicationRoleCreatedEvent(string roleId, string roleName) : ApplicationRoleEvent(roleId, roleName);
