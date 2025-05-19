using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Domain.Entities.Identity;

public class ApplicationUserClaim : IdentityUserClaim<Guid>
{
    public string? TenantId { get; set; }
}
