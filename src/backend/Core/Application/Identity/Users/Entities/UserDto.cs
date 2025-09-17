namespace EvrenDev.Application.Identity.Users.Entities;

public class UserDto
{
    public Guid Id { get; set; }
    public Gender? Gender { get; set; }
    public Language? Language { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName => $"{FirstName} {LastName}".Trim();

    public string Initial => !string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(LastName)
        ? $"{FirstName?[0].ToString().ToUpper()}{LastName?[0].ToString().ToUpper()}"
        : string.Empty;

    public DateTime? Birthday { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public string? PhoneNumber { get; set; }
    public bool TwoFactorEnabled { get; set; } = false;
}
