namespace EvrenDev.Application.Identity.Users;

public class UserDetailsDto
{
    public Guid Id { get; set; }
    public string? Gender { get; set; }
    public string? Language { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName => $"{FirstName} {LastName}".Trim();
    public string Initial => !string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(LastName) ? $"{FirstName?[0]}{LastName?[0]}" : string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ImageUrl { get; set; }
    public bool TwoFactorEnabled { get; set; } = false;
}
