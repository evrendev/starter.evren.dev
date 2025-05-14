using EvrenDev.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Domain.Entities.Identity;

[AuditInclude]
public class ApplicationUser : IdentityUser<Guid>, IBaseIdentityEntity
{
    public string? TenantId { get; set; }

    public Gender? Gender { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Image { get; set; }

    public string? JobTitle { get; set; }

    public Language? Language { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public DateTimeOffset CreatedTime { get; set; }

    public string? Creator { get; set; }

    public DateTimeOffset ModifiedTime { get; set; }

    public string? Modifier { get; set; }

    public bool Deleted { get; set; }

    public string? Deleter { get; set; }

    public DateTimeOffset? DeletionTime { get; set; }

    public void Restore()
    {
        Deleted = false;
        Deleter = null;
        DeletionTime = null;
    }

    public void Delete(string? deleter)
    {
        Deleted = true;
        Deleter = deleter;
        DeletionTime = DateTimeOffset.Now;
    }
}
