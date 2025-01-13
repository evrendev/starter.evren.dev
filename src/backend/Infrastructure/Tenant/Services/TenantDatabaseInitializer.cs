using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Tenant;
using EvrenDev.Infrastructure.Tenant.Data;
using Microsoft.Extensions.Configuration;

namespace EvrenDev.Infrastructure.Tenant.Services;

public class TenantDatabaseInitializer : IDatabaseSeeder
{
    private readonly TenantDbContext _context;
    private readonly IConfiguration _configuration;

    public TenantDatabaseInitializer(
        TenantDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }

            if (_context.Tenants != null && !await _context.Tenants.AnyAsync())
            {
                await SeedTenantsAsync();
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task SeedTenantsAsync()
    {
        if (_context.Tenants == null) return;
        var defaultTenant = _configuration.GetSection("DefaultTenant").Get<TenantEntity>();

        if (defaultTenant == null) return;

        await _context.Tenants.AddAsync(defaultTenant);
    }
}
