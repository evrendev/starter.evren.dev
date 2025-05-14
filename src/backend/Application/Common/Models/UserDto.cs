using EvrenDev.Shared.Constants;

namespace EvrenDev.Application.Common.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public string? TenantId { get; set; }
    public string? Gender { get; set; } = Defaults.Gender.Code;
    public string Initial => !string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(LastName) ? $"{FirstName?[0]}{LastName?[0]}" : string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Image { get; set; } = null;
    public string JobTitle { get; set; } = string.Empty;
    public string? Language { get; set; } = Defaults.Language.Code;
    public List<string> Permissions { get; set; } = [];
    public bool TwoFactorEnabled { get; set; } = false;
}
