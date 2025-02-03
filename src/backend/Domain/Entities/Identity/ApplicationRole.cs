using EvrenDev.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Domain.Entities.Identity;

[AuditInclude]
public class ApplicationRole : IdentityRole<Guid>, ITenant
{
    public Guid? TenantId { get; set; }

    public string? Description { get; set; }

    public bool Deleted { get; set; } = false;
}
