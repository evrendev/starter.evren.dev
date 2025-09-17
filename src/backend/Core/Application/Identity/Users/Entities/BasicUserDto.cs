using System.Text.Json.Serialization;

namespace EvrenDev.Application.Identity.Users.Entities;

public class BasicUserDto
{
    public Guid Id { get; set; }

    [JsonIgnore] public string? FirstName { get; set; }

    [JsonIgnore] public string? LastName { get; set; }

    public string? FullName => $"{FirstName} {LastName}".Trim();

    public string Initial => !string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(LastName)
        ? $"{FirstName?[0].ToString().ToUpper()}{LastName?[0].ToString().ToUpper()}"
        : string.Empty;

    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public bool TwoFactorEnabled { get; set; } = false;
}
