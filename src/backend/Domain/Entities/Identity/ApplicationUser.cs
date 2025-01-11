using EvrenDev.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser, ITenant
{
    public string? TenantId { get; set; }

    public Gender? Gender { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Image { get; set; }

    public string? JobTitle { get; set; }

    public Language? Language { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public bool Deleted { get; set; } = false;
}
