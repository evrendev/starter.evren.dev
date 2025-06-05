namespace EvrenDev.Domain.Common.Events.Identity;

public class ApplicationRoleCreatedEvent(string roleId, string roleName) : ApplicationRoleEvent(roleId, roleName);
