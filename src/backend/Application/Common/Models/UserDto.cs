using EvrenDev.Shared.Constants;

namespace EvrenDev.Application.Common.Models;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public Gender? Gender { get; set; } = Defaults.Gender;
    public string Initial => $"{FirstName[0]}{LastName[0]}";
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Image { get; set; } = null;
    public string JobTitle { get; set; } = string.Empty;
    public Language? Language { get; set; } = Defaults.Language;
    public List<string> Permissions { get; set; } = new();
}
