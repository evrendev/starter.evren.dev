using EvrenDev.Shared.Constants;
using EvrenDev.Shared.ValueObjects;

namespace EvrenDev.Application.Common.Models;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public Language? Language { get; set; } = Defaults.Language;
    public string? TenantId { get; set; }
    public bool Deleted { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}
