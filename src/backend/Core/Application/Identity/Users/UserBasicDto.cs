namespace EvrenDev.Application.Identity.Users;

public class UserBasicDto
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? FullName => $"{FirstName} {LastName}".Trim();

    public bool TwoFactorEnabled { get; set; } = false;
}
