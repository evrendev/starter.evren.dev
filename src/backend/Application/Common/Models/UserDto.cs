using EvrenDev.Shared.Constants;

namespace EvrenDev.Application.Common.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string? Gender { get; set; } = Defaults.Gender.Code;
    public string Initial => $"{FirstName[0]}{LastName[0]}";
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Image { get; set; } = null;
    public string JobTitle { get; set; } = string.Empty;
    public string? Language { get; set; } = Defaults.Language.Code;
    public List<string> Permissions { get; set; } = [];
}
