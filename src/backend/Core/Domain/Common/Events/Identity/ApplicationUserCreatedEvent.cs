namespace EvrenDev.Domain.Common.Events.Identity;

public class ApplicationUserCreatedEvent(string userId) : ApplicationUserEvent(userId);
