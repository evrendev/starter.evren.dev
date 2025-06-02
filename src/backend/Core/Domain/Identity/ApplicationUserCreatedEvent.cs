namespace EvrenDev.Domain.Identity;

public class ApplicationUserCreatedEvent(string userId) : ApplicationUserEvent(userId);
