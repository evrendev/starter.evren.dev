namespace EvrenDev.Domain.Common.Events.Identity;

public class ApplicationUserUpdatedEvent(string userId, bool rolesUpdated = false) : ApplicationUserEvent(userId)
{
    public bool RolesUpdated { get; set; } = rolesUpdated;
}
