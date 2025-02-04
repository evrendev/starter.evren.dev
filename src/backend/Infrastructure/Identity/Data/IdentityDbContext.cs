using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Identity;
using EvrenDev.Infrastructure.Identity.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EvrenDev.Infrastructure.Identity.Data;

public class IdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    private readonly ITenantService _tenantService;

    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options,
        ITenantService tenantService)
        : base(options)
    {
        _tenantService = tenantService;
    }

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

        builder.Entity<ApplicationUser>()
            .HasQueryFilter(u => !u.Deleted && u.TenantId == _tenantService.GetCurrentTenantId());

        builder.Entity<ApplicationRole>()
            .HasQueryFilter(r => !r.Deleted);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<ApplicationUser>().Where(e => e.State == EntityState.Added))
        {
            var tenantId = _tenantService.GetCurrentTenantId();

            entry.Entity.TenantId = tenantId ?? entry.Entity.TenantId;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
