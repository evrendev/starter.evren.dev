namespace EvrenDev.Application.Features.Users.Models;

public class BasicUserDto
{
    public Guid Id { get; set; }
    public string? Tenant { get; set; }
    public string? Gender { get; set; }
    public string Initial => !string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(LastName) ? $"{FirstName?[0]}{LastName?[0]}" : string.Empty;
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool Deleted { get; set; }
}
