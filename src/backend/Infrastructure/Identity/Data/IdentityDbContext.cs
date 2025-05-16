using EvrenDev.Domain.Entities.Identity;
using EvrenDev.Infrastructure.Identity.Configurations;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;

namespace EvrenDev.Infrastructure.Identity.Data;

public class IdentityDbContext : MultiTenantIdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public IdentityDbContext(IMultiTenantContextAccessor multiTenantContextAccessor,
        DbContextOptions<IdentityDbContext> options)
        : base(multiTenantContextAccessor, options)
    { }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("Identity");

        builder.ApplyConfiguration(new ApplicationRoleConfiguration());
        builder.ApplyConfiguration(new ApplicationUserConfiguration());
        builder.ApplyConfiguration(new IdentityRoleClaimConfiguration());
        builder.ApplyConfiguration(new IdentityUserClaimConfiguration());
        builder.ApplyConfiguration(new IdentityUserLoginConfiguration());
        builder.ApplyConfiguration(new IdentityUserRoleConfiguration());
        builder.ApplyConfiguration(new IdentityUserTokenConfiguration());

        builder.Entity<ApplicationUser>().HasQueryFilter(u => !u.Deleted);
        builder.Entity<ApplicationRole>().HasQueryFilter(r => !r.Deleted);
    }
}
