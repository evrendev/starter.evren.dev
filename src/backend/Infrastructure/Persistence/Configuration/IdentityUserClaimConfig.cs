using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class IdentityUserClaimConfig : IEntityTypeConfiguration<IdentityUserClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
    {
        builder
            .ToTable("UserClaims", SchemaNames.Identity)
            .IsMultiTenant();
    }
}
