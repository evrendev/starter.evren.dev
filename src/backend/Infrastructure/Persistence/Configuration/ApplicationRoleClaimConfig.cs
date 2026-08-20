using EvrenDev.Infrastructure.Identity;
using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class ApplicationRoleClaimConfig : IEntityTypeConfiguration<ApplicationRoleClaim>
{
    public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
    {
        builder
            .ToTable("RoleClaims", SchemaNames.Identity)
            .IsMultiTenant();
    }
}
