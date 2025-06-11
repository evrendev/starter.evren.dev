namespace EvrenDev.Application.Identity.Users;

public class UserBasicDto
{
    public string? Id { get; set; }
    public string? Gender { get; set; }
    public string? Language { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Initial => !string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(LastName) ? $"{FirstName?[0]}{LastName?[0]}" : string.Empty;
    public string? Email { get; set; }
    public string? FullName => $"{FirstName} {LastName}".Trim();
    public bool TwoFactorEnabled { get; set; } = false;
}
